import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AccountSerVice } from '@app/_services/account.service';
import { MessageService, PrimeNGConfig, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { AddAccountComponent } from './add-account/add-account.component';
import { EditAccountComponent } from './edit-account/edit-account.component';
import { ImportAccountComponent } from './import-account/import-account.component';

@Component({
  selector: 'app-account-list',
  templateUrl: './account-list.component.html',
  styleUrls: ['./account-list.component.scss'],
  providers: [ConfirmationService]
})
export class AccountListComponent implements OnInit {

  accounts: any = [];
  loading!: boolean;
  keyWord: any;
  @ViewChild('dt') table!: Table;

  constructor(private accountService: AccountSerVice, private confirmationService: ConfirmationService, private primengConfig: PrimeNGConfig, private messageService: MessageService, private matDialog: MatDialog) {
  }

  getListAccountInformation() {
    this.accountService.getListAccountInformation().subscribe((res: any) => {
      this.accounts = res.data;
    });
  }

  ngOnInit(): void {
    this.loading = true;
    setTimeout(() => {
      this.getListAccountInformation();
      this.loading = false;
    }, 1000);
    this.primengConfig.ripple = true;
  }

  deleteAccount(userName: string, x, userId: string) {
    this.confirmationService.confirm({
      message: "Are you want to delete " + userName + "?",
      header: "Delete Confirmation",
      icon: "pi pi-info-circle",
      accept: () => {
        // this.accounts.splice(x, 1);
        this.accountService.deleteAccount(userId).subscribe((res: any) => {
          this.accounts = res.data;
          this.getListAccountInformation();
        });
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Delete Account successfully' });
      },
      reject: () => {
        return;
      }
    })
  }

  showConfirm(userName: string) {
    this.messageService.clear();
    this.messageService.add({ key: 'cd', sticky: true, severity: 'warn', summary: 'Are you sure you want to delete ' + userName, detail: 'Confirm to delete' });
  }

  onReject() {
    this.messageService.clear('cd');
  }

  searchAccountKeyDown(event) {
    if (event.keyCode === 13 || event.key === "Enter") {
      if (this.keyWord == null || this.keyWord == '') {
        this.getListAccountInformation();
      }
      else {
        this.accountService.searchAccountByUsername(this.keyWord).subscribe((res: any) => {
          this.accounts = res.data;
        });
      }
    }
  }

  searchAccountBtn() {
    if (this.keyWord == null || this.keyWord == '') {
      this.getListAccountInformation();
    }
    else {
      this.accountService.searchAccountByUsername(this.keyWord).subscribe((res: any) => {
        this.accounts = res.data;
      });
    }
  }

  openEditAccoutDialog(userID: string) {
    let dialogRef = this.matDialog.open(EditAccountComponent, { data: { id: userID }, autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.getListAccountInformation();
    });
  }

  openImportAccoutDialog() {
    let dialogRef = this.matDialog.open(ImportAccountComponent);
    dialogRef.afterClosed().subscribe(result => {
      this.getListAccountInformation();
    });
  }

  openAddAccoutDialog() {
    let dialogRef = this.matDialog.open(AddAccountComponent, { autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.getListAccountInformation();
    });
  }

}
