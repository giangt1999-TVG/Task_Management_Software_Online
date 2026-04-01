import { ClassGetter } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AccountSerVice } from '@app/_services/account.service';
import { MessageService } from 'primeng/api';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-import-account',
  templateUrl: './import-account.component.html',
  styleUrls: ['./import-account.component.css']
})
export class ImportAccountComponent implements OnInit {

  uploadedFiles: any[] = [];
  accounts: any = [];
  errors: any;
  convertedJson!: string;

  constructor(private accountService: AccountSerVice, private messageService: MessageService, private matDialog: MatDialog) { }

  ngOnInit(): void {
  }

  onUpload(event: any) {
    const selectedFile = event.files[0];
    const fileReader = new FileReader();
    fileReader.readAsBinaryString(selectedFile);
    fileReader.onload = (event) => {
      let binaryData = event.target?.result;
      let workbook = XLSX.read(binaryData, { type: 'binary' })
      workbook.SheetNames.forEach(sheet => {
        const data = XLSX.utils.sheet_to_json(workbook.Sheets[sheet]);
        if (data.length >= 1000) {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: 'Maximum row is 1000' });
          return;
        }
        else if (data.length == 0) {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: 'No acounts found in excel file' });
          return;
        }
        else {
          this.convertedJson = JSON.stringify(data)
        }
      })
      this.accountService.insertAccountFile(this.convertedJson).subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        for (let e of res.data.failuresRecords) {
          this.messageService.add({ key: 'tr', severity: 'warn', summary: 'File Uploaded', detail: 'Duplicate Email: ' + e.email, sticky: true });
        }
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'File Uploaded', detail: 'Upload File Successfully' });
      },
        (error) => {
          this.errors = error.error.errors;
          for (let [key, value] of Object.entries(this.errors)) {
            this.messageService.add({ key: 'error-message', severity: 'error', summary: 'Error', detail: 'Line ' + key + ': ' + value, sticky: true })
          }
          // Object.entries(this.error).forEach(
          //   ([key, value]) =>
          //   this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail:'Line ' + key + ': ' + value, sticky: true })
          // );
        });
    }
  }

}
