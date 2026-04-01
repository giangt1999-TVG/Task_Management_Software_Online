import { MessageService } from 'primeng/api';
import { ProjectSerVice } from './../../../_services/project.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from './../../../_services/user.service';
import { Component, Inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-add-member',
  templateUrl: './add-member.component.html',
  styleUrls: ['./add-member.component.css']
})
export class AddMemberComponent implements OnInit {

  addNewMemberForm!: FormGroup;
  students: any[] = [];
  studentSelected: any;
  submitted = false;
  newMemberSelectedId!: string;
  listUser: any = [];
  listMember: any[] = [];
  projectId: any;
  teamLeadId: any;

  constructor(@Inject(MAT_DIALOG_DATA) data, private matDialog: MatDialog, private formBuilder: FormBuilder, private userService: UserService, private projectService: ProjectSerVice, private messageService: MessageService) {
    this.listUser = data.listUser;
    this.projectId = data.projectId;
  }

  ngOnInit(): void {
    this.addNewMemberForm = this.formBuilder.group({
      studentSelected: [],
    })
    this.getAllStudent();
  }

  // Convenience getter for easy access to form fields
  get f() { return this.addNewMemberForm.controls; }

  // Get all student
  getAllStudent() {
    this.userService.getAllStudent().subscribe((res: any) => {
      this.students = res.data;
    });
  }

  // Get teamlead
  getTeamleadInProject() {
    for (let index = 0; index < this.listUser.length; index++) {
      if (this.listUser[index].role === "Teamlead") {
        this.teamLeadId = this.listUser[index].userId;
      }
    }
    return this.teamLeadId;
  }

  //Check student id is exist
  checkValidStudentId() {
    this.getListMemberInProject();
    for (let index = 0; index < this.listMember.length; index++) {
      if (this.f.studentSelected.value[0].id !== null && this.f.studentSelected.value[0].id !== this.listMember[index]) {
        this.newMemberSelectedId = this.f.studentSelected.value[0].id;
      }
      else {
        return this.newMemberSelectedId = "";
      }
    }
    return this.newMemberSelectedId;
  }

  //Get list member
  getListMemberInProject() {
    for (let index = 0; index < this.listUser.length; index++) {
      if (this.listUser[index].role !== "Teacher") {
        if (this.listUser[index].role !== "Teamlead") {
          this.listMember.push(this.listUser[index].userId);
        }
      }
    }
    return this.listMember;
  }

  //Add new member
  addNewMember() {
    this.submitted = true;
    if (this.addNewMemberForm.invalid) {
      return;
    }
    else {
      this.checkValidStudentId();
      this.getTeamleadInProject();
      if (this.newMemberSelectedId !== "" && this.newMemberSelectedId !== this.teamLeadId) {
        this.projectService.createNewMemberProject(this.projectId, this.newMemberSelectedId).subscribe((res: any) => {
          this.students = res.data;
          this.matDialog.closeAll();
        });
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Add New Member successfully' });
      }
      else if (this.newMemberSelectedId !== "" && this.newMemberSelectedId === this.teamLeadId) {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Member is existed!' });
        return;
      }
      else {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Member is invalid!' });
        return;
      }
    }
  }

  closeDialog() {
    this.matDialog.closeAll();
  }

}
