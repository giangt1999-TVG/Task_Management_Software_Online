import { UserService } from './../../../_services/user.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ProjectSerVice } from '@app/_services/project.service';
import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-update-teacher',
  templateUrl: './update-teacher.component.html',
  styleUrls: ['./update-teacher.component.css']
})
export class UpdateTeacherComponent implements OnInit {

  updateNewTeacherForm!: FormGroup;
  projectId: any;
  userId: any;
  role: any;
  teachers: any[] = [];
  teacherSelected: any;
  submitted = false;

  constructor(@Inject(MAT_DIALOG_DATA) data, private matDialog: MatDialog, private formBuilder: FormBuilder, private userService: UserService, private projectService: ProjectSerVice, private messageService: MessageService) {
    this.projectId = data.projectId;
    this.userId = data.userId;
    this.role = data.role;
  }

  ngOnInit(): void {
    this.updateNewTeacherForm = this.formBuilder.group({
      teacherSelected: [],
    })
    this.getAllTeacher();
  }

  // Get all teacher
  getAllTeacher() {
    this.userService.getAllTeacher().subscribe((res: any) => {
      this.teachers = res.data;
    });
  }

  // Update teacher
  updateTeacher() {
    this.submitted = true;
    if (this.updateNewTeacherForm.invalid) {
      return;
    }
    else {
      if(this.f.teacherSelected.value[0].id !== "" && this.f.teacherSelected.value[0].id !== this.userId){
        this.projectService.updateTeacherAndTeamleadInProject(this.projectId, this.userId, this.f.teacherSelected.value[0].id, this.role).subscribe((res: any) => {
          this.teachers = res.data;
          this.matDialog.closeAll();
        });
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update New Teacher successfully' });
      }
      else if(this.f.teacherSelected.value[0].id !== "" && this.f.teacherSelected.value[0].id === this.userId){
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Current member is teacher!' });
        return;
      }
      else {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Member is invalid!' });
        return;
      }
    }
  }

  // Convenience getter for easy access to form fields
  get f() { return this.updateNewTeacherForm.controls; }

  closeDialog() {
    this.matDialog.closeAll();
  }
  
}
