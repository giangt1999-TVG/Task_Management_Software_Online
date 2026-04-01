import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import {MessageService} from 'primeng/api';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountSerVice } from '@app/_services/account.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class ChangePasswordComponent implements OnInit {
  confirmPassword!: string;
  newPassword!: string;
  currentPassword!: string;
  changePasswordForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  constructor(
    private _modal: DynamicDialogRef,
    private _config: DynamicDialogConfig,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private accountService: AccountSerVice,
  ) { 

  }

  ngOnInit(): void {
    this.changePasswordForm = this.formBuilder.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.pattern(/^(((?=.*[a-z])(?=.*[A-Z]))|((?=.*[a-z])(?=.*[0-9]))|((?=.*[A-Z])(?=.*[0-9])))(?=.{6,})./)]],
      confirmPassword: ['', [Validators.required]]
    });
  }

  // convenience getter for easy access to form fields
  get f() { return this.changePasswordForm.controls; }

  closeModal() {
    this._modal.close();
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.changePasswordForm.invalid) {
        return;
    }

    if (this.f.newPassword.value != this.f.confirmPassword.value) {
      this.error = 'The password and confirmation password do not match.'
      return;
    }

    this.accountService.changePassword(this._config.data.userId, this.f.currentPassword.value, this.f.confirmPassword.value).subscribe((res: any) => {
      this.closeModal();
      this.messageService.add({key: 'tr', severity:'success', summary:'Success', detail: 'Password changed!'});
    },
    (err: any) => {
      if (err.status === 400) {
        this.error = err.error[0].description;
      }
    });
  }

}
