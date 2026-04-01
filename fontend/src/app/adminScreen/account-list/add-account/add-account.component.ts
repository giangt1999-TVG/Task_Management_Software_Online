import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AccountSerVice } from '@app/_services/account.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-add-account',
  templateUrl: './add-account.component.html',
  styleUrls: ['./add-account.component.css']
})
export class AddAccountComponent implements OnInit {

  accounts: any = [];
  roles: any = [];
  selectedRole: any;
  userName!: string;
  error = "";
  submitted = false;
  isDuplicate = false;
  // userNameValue = "";
  addForm!: FormGroup;
  // disableBtn: boolean = false;

  constructor(private accountService: AccountSerVice, private formBuilder: FormBuilder, private messageService: MessageService, private matDialog: MatDialog) { }

  // getAccountInfoById() {
  //   this.accountService.getUserInfoById(this.data.id).subscribe((res: any) => {
  //     this.accounts = res.data;
  //   });
  // }

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

  addAccount() {
    this.submitted = true;
    this.getCheckRollNumberDuplicate();
    if (this.isDuplicate = true){
      this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: 'Roll Number already exist' });
    }
    else if (this.f.fullName.value == null || this.f.rollNumber.value == null || this.f.email.value == null) {
      return;
    }
    else if (this.addForm.invalid) {
      return;
    }
    // else if (this.f.selectedRoleName.value == null) {
    //   this.accountService.updateAccountInfo(userID, this.f.fullName.value, this.f.userName.value, null, this.f.rollNumber.value).subscribe((res: any) => {
    //     this.accounts = res.data;
    //   });
    //   this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update Account successfully' });
    // }

    else if (this.f.rollNumber.value == null) {
      this.accountService.addAccount(this.f.fullName.value, this.f.email.value, this.f.selectedRoleName.value.name, "").subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Add Account successfully' });
      },
        (error) => {
          this.error = error;
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error.error });
        });
    }

    else {
      this.accountService.addAccount(this.f.fullName.value, this.f.email.value, this.f.selectedRoleName.value.name, this.f.rollNumber.value).subscribe((res: any) => {
        this.accounts = res.data;
        this.matDialog.closeAll();
        this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Add Account successfully' });
      },
        (error) => {
          // this.error = error;
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error.error });
        });
      // this.matDialog.closeAll();
    }
  }

  ngOnInit(): void {
    this.getAllRole();

    this.addForm = this.formBuilder.group({
      fullName: ['', [
        Validators.required,
        Validators.maxLength(50),
        Validators.pattern(/[a-zA-Z\sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệóòỏõọôốồổỗộơớờởỡợíìỉĩịúùủũụưứừửữựýỳỷỹỵđ]$/)
      ]],
      rollNumber: ['', [
        Validators.maxLength(8),
        Validators.pattern(/[a-zA-Z0-9]$/)
      ]],
      email: ['', [
        Validators.required,
        Validators.maxLength(100),
        Validators.email
      ]],
      selectedRoleName: [, [
        Validators.required
      ]]
    });
  }

  get f() { return this.addForm.controls; }

}
