import { MessageService } from 'primeng/api';
import { ProjectSerVice } from '@app/_services/project.service';
import { UserService } from './../../../_services/user.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Component, Inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-update-teamlead',
  templateUrl: './update-teamlead.component.html',
  styleUrls: ['./update-teamlead.component.css']
})
export class UpdateTeamleadComponent implements OnInit {

  updateNewTeamleadForm!: FormGroup;
  projectId: any;
  userId: any;
  role: any;
  listStudentsMember: any[] = [];
  teamleadSelected: any;
  submitted = false;
  listUser: any = [];

  constructor(@Inject(MAT_DIALOG_DATA) data, private matDialog: MatDialog, private formBuilder: FormBuilder, private userService: UserService, private projectService: ProjectSerVice, private messageService: MessageService) { 
    this.projectId = data.projectId;
    this.userId = data.userId;
    this.role = data.role;
    this.listUser = data.listUser;
  }

  ngOnInit(): void {
    this.updateNewTeamleadForm = this.formBuilder.group({
      teamleadSelected: [],
    });
    this.getAllStudent();
    this.getListStudentMember();
    console.log(this.listUser);
  }

  //Get all student
  getAllStudent(){
    this.userService.getAllStudent().subscribe((res: any) => {
      this.listStudentsMember = res.data;
    });
  }
  
  // Get list student member
  getListStudentMember(){
    for (let index = 0; index < this.listUser.length; index++) {
      if (this.listUser[index].role !== "Teacher") {
        if (this.listUser[index].role !== "Teamlead"){
          this.listStudentsMember.push(this.listUser[index]);
        }
      }
    }
    console.log(this.listStudentsMember);
    return this.listStudentsMember;
  }

  // Update teamlead
  updateTeamlead(){
    this.submitted = true;
    if (this.updateNewTeamleadForm.invalid) {
      return;
    }
    else {
      if(this.f.teamleadSelected.value[0].id!== "" && this.f.teamleadSelected.value[0].id !== this.userId){
        this.projectService.updateTeacherAndTeamleadInProject(this.projectId, this.userId, this.f.teamleadSelected.value[0].id, this.role).subscribe((res: any) => {
          this.listStudentsMember = res.data;
          this.matDialog.closeAll();
        });
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update New Teacher successfully' });
      }
      else if(this.f.teamleadSelected.value[0].id !== "" && this.f.teamleadSelected.value[0].id === this.userId){
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Current member is teamlead!' });
        return;
      }
      else {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Member is invalid!' });
        return;
      }
    }
  }

  // Convenience getter for easy access to form fields
  get f() { return this.updateNewTeamleadForm.controls; }

  closeDialog() {
    this.matDialog.closeAll();
  }

}
