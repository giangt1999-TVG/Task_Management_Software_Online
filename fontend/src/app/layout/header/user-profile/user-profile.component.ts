import { AuthenticationService } from '@app/_services/authentication.service';
import { MessageService } from 'primeng/api';
import { UploadImageService } from './../../../shared/upload-image.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from './../../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { Role } from '@app/_models/role';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {

  submitted = false;
  userInformationForm!: FormGroup;
  imageUrl!: string;
  inputfullName!: string;
  uploadNewFile!: File;
  userInfo!: User;
  inputDescription!: string;
  isDeleted!: boolean;
  id!: string;
  shortName!: string | undefined;

  constructor(private userService: UserService,
    private formBuilder: FormBuilder,
    private uploadFileService: UploadImageService,
    private matDialog: MatDialog,
    private messageService: MessageService,
    private authenticationService: AuthenticationService,
    public dialogRef: MatDialogRef<UserProfileComponent>) {
  }

  ngOnInit(): void {
    this.getUserInfo();
    this.userInformationForm = this.formBuilder.group({
      id: [],
      inputfullName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50), Validators.pattern(/[a-zA-Z\sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệóòỏõọôốồổỗộơớờởỡợíìỉĩịúùủũụưứừửữựýỳỷỹỵđ]$/)]],
      imageUrl: [],
      inputDescription: ['', [Validators.minLength(0), Validators.maxLength(1000)]],
      isDeleted: false,
    });
  }

  //Select File to Upload
  //Show image preview
  selectedFile(event) { // called each time file input changes
    if (event.target.files) {
      this.uploadNewFile = event.target.files[0];
      if (this.uploadNewFile.type.includes('image')) {
        var reader = new FileReader();
        reader.readAsDataURL(event.target.files[0]);  // read file as data url
        reader.onload = (event: any) => { // called once readAsDataURL is completed
          this.userInfo.avatarUrl = event.target.result;
        }
        this.upLoadImageFile();
      }
      else {
        this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: 'File upload is not image file!' });
        return;
      }
    }
  }

  //API upload image file
  upLoadImageFile() {
    this.uploadFileService.uploadImageFile(this.uploadNewFile).subscribe((res: any) => {
      this.imageUrl = res.data;
    }
    );
  }

  //API get User Info
  getUserInfo() {
    this.userService.getUserInfoById().subscribe((res: any) => {
      this.userInfo = res.data;
      this.shortName = this.getShortName(this.userInfo.fullName);
    });
  }

  getIdUser() {
    if (this.userInfo.id !== null) {
      this.id = this.userInfo.id;
    }
    return this.id;
  }

  // API update information user
  updateInfomationUser() {
    this.getIdUser();
    this.userService.updateInfomationUserAPI(this.getIdUser(), this.f.inputfullName.value.trim(), this.imageUrl, this.f.inputDescription.value.trim(), this.isDeleted).subscribe((res: any) => {
      this.userInfo = res.data;
      this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Update User Profile successfully' });
    });
    this.onCloseClick();
    this.getUserInfo();
  }

  // Convenience getter for easy access to form fields
  get f() { return this.userInformationForm.controls; }

  saveChangesInfo() {
    this.submitted = true;
    if (this.userInformationForm.invalid) {
      return;
    }
    else {
      this.userInformationForm.value;
      this.updateInfomationUser();
    }
  }

  private getShortName(text) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }
  onCloseClick() {
    this.dialogRef.close({ event: 'close', data: true });
  }
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  avatarUrl: string;
  role: Role;
  description: string;
  isDeleted: boolean;
}