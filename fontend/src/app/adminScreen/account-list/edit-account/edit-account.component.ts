import { AccountListComponent } from './../account-list.component';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { AccountSerVice } from '@app/_services/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.css']
})
export class EditAccountComponent implements OnInit {

  accounts: any = [];
  roles: any = [];
  selectedRole: any;
  userName!: string;
  submitted = false;
  required = false;
  isDuplicate = false;
  editForm!: FormGroup;

  constructor(private accountService: AccountSerVice,
    private formBuilder: FormBuilder,
    private messageService: MessageService,
    private matDialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { id: string }) { }

  async getAccountInfoById() {
    await this.accountService.getUserInfoById(this.data.id).then((res: any) => {
      this.accounts = res.data;
    });
  }

  getAllRole() {
    this.accountService.getListRoles().subscribe((res: any) => {
      this.roles = res.data;
    });
  }

  getCheckRollNumberDuplicate() {
    this.accountService.getListAccountInformation().subscribe((res: any) => {
      this.accounts = res.data;
      this.accounts.map(o => {
        if (this.f.rollNumber.value === o.rollNumber) {
          this.isDuplicate = true;
        }
      })
    });
  }

  updateAccount(userID: string) {
    this.submitted = true;
    // don't edit anything
    if (this.isDuplicate = true) {
      this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: 'Roll Number already exist' });
      return;
    }
    else if (this.editForm.invalid) {
      return;
    }
    else if (this.f.rollNumber.value == "") {
      this.accountService.updateAccountInfo(userID, this.f.fullName.value, this.f.userName.value, " ").subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.getAccountInfoById();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update Account successfully' });
      },
        (error) => {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error });
        });
    }
    else if (this.f.fullName.value == null) {
      this.accountService.updateAccountInfo(userID, null, this.f.userName.value, this.f.rollNumber.value).subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.getAccountInfoById();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update Account successfully' });
      },
        (error) => {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error });
        });
    }
    else if (this.f.userName.value == null) {
      this.accountService.updateAccountInfo(userID, this.f.fullName.value, null, this.f.rollNumber.value).subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.getAccountInfoById();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update Account successfully' });
      },
        (error) => {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error });
        });
    }
    else if (this.f.fullName.value == null && this.f.userName.value == null) {
      this.accountService.updateAccountInfo(userID, null, null, this.f.rollNumber.value).subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.getAccountInfoById();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update Account successfully' });
      },
        (error) => {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error });
        });
    }
    //edit all
    else {
      this.accountService.updateAccountInfo(userID, this.f.fullName.value, this.f.userName.value, this.f.rollNumber.value).subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.getAccountInfoById();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update Account successfully' });
      },
        (error) => {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error });
        });
    }

  }

  ngOnInit(): void {
    this.getAccountInfoById();
    this.getAllRole();

    this.editForm = this.formBuilder.group({
      userName: [null, [
        // Validators.required,
        Validators.maxLength(50),
        Validators.pattern(/[a-zA-Z0-9]$/)
      ]],
      fullName: [null, [
        // Validators.required,
        Validators.maxLength(50),
        Validators.pattern(/[a-zA-Z\sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệóòỏõọôốồổỗộơớờởỡợíìỉĩịúùủũụưứừửữựýỳỷỹỵđ]$/)
      ]],
      rollNumber: ["", [
        Validators.maxLength(8),
        Validators.pattern(/[a-zA-Z0-9]$/)
      ]]
    });
  }

  get f() { return this.editForm.controls; }

}
