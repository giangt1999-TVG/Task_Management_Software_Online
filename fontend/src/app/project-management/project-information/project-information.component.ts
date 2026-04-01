import { UpdateTeamleadComponent } from './update-teamlead/update-teamlead.component';
import { UpdateTeacherComponent } from './update-teacher/update-teacher.component';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthenticationService } from '@app/_services/authentication.service';
import { User } from '@app/_models/user';
import { LabelManagementComponent } from './label-management/label-management.component';
import { ActivatedRoute } from '@angular/router';
import { AddMemberComponent } from './add-member/add-member.component';
import { MatDialog } from '@angular/material/dialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ProjectSerVice } from '@app/_services/project.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-project-information',
  templateUrl: './project-information.component.html',
  styleUrls: ['./project-information.component.scss'],
  providers: [ConfirmationService]
})
export class ProjectInformationComponent implements OnInit {

  editprojectInformationForm!: FormGroup;
  submitted = false;
  startDate!: Date;
  endDate!: Date;
  project: any = [];
  currentUser!: User | null;
  teamLeadId!: string;

  constructor(private formBuilder: FormBuilder, private activatedRoute: ActivatedRoute, private projectService: ProjectSerVice, private messageService: MessageService, private confirmationService: ConfirmationService, private matDialog: MatDialog, private author: AuthenticationService) {
    this.author.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    this.editprojectInformationForm = this.formBuilder.group({
      projectCode: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150), Validators.pattern(/([a-zA-Z0-9])$/)]],
      projectName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(250), Validators.pattern(/([a-zA-Z0-9])$/)]],
      startDate: [],
      endDate: [],
      description: ['', [Validators.minLength(0), Validators.maxLength(1000)]],
    });
    this.getProjectInformation();
  }

  //Get start date
  getStartDate() {
    if (this.project.startDate) {
      this.startDate = new Date(this.project.startDate);
    }
    return this.startDate;
  }

  //Get end date
  getEndDate() {
    if (this.project.endDate) {
      this.endDate = new Date(this.project.endDate);
    }
    return this.endDate;
  }

  //Get id of teamlead project
  getTeamleadIdInProject() {
    for (let index = 0; index < this.project.listUser.length; index++) {
      if (this.project.listUser[index].role === "Teamlead") {
        this.teamLeadId = this.project.listUser[index].userId;
      }
    }
    return this.teamLeadId;
  }

  // Check user with role teamlead
  // checkUserId() {
  //   this.getTeamleadIdInProject();
  //   if (this.currentUser?.id === this.teamLeadId) {
  //     return true;
  //   }
  //   else {
  //     return false;
  //   }
  // }

  // Get project information
  getProjectInformation() {
    let projectid = this.activatedRoute.snapshot.params.id;
    this.projectService.getProjectInformation(projectid).subscribe((res: any) => {
      this.project = res.data;
      this.getStartDate();
      this.getEndDate();
      this.getTeamleadIdInProject();
    });
  }

  //Open Add new member dialog
  openAddNewMemberDialog(projectId: string, listUser: any = []) {
    let dialogRef = this.matDialog.open(AddMemberComponent, { data: { projectId: projectId, listUser: listUser }, autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.getProjectInformation();
    });
  }

  //Open Add new label dialog
  openAddNewLabelDialog(projectId: string) {
    let dialogRef = this.matDialog.open(LabelManagementComponent, { data: { projectId: projectId }, autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.getProjectInformation();
    });
  }

  // API update teamlead
  updateNewTeamlead(userId, role, fullName, listUser: any = []) {
    let projectId = this.activatedRoute.snapshot.params.id;
    let dialogRef = this.matDialog.open(UpdateTeamleadComponent, { data: { projectId: projectId, userId: userId, role: role, listUser: listUser }, autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.getProjectInformation();
    });
  }

  // API update teacher
  updateNewTeacher(userId: string, role: string, fullName: string) {
    let projectId = this.activatedRoute.snapshot.params.id;
    let dialogRef = this.matDialog.open(UpdateTeacherComponent, { data: { projectId: projectId, userId: userId, role: role }, autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.getProjectInformation();
    });
  }

  // Delete Member
  deleteMember(userId, role, fullName) {
    let projectid = this.activatedRoute.snapshot.params.id;
    // Do not delete teacher
    if (role == "Teacher") {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Not allowed to delete teacher.' })
    }
    // Do not delete teamlead
    else if (role == "Teamlead") {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Not allowed to delete teamlead.' })
    }
    else {
      this.confirmationService.confirm({
        message: "Are you want to delete " + fullName + "?",
        header: "Delete Confirmation",
        icon: "pi pi-info-circle",
        accept: () => {
          this.projectService.deleteProjectMember(projectid, userId).subscribe((res: any) => {
            this.project = res.data;
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Delete student member successfully.' });
            this.getProjectInformation();
          });
        },
        reject: () => {
          return;
        }
      })
    }
  }

  //Delete Label
  deleteLabel(labelId: string, name: string) {
    let projectid = this.activatedRoute.snapshot.params.id;
    this.confirmationService.confirm({
      message: "Are you want to delete " + name + " label?",
      header: "Delete Confirmation",
      icon: "pi pi-info-circle",
      accept: () => {
        this.projectService.deleteProjectLabel(projectid, labelId).subscribe((res: any) => {
          this.project = res.data;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Delete label successfully.' });
          this.getProjectInformation();
        });
      },
      reject: () => {
        return;
      }
    })
  }

  //API update information project
  updateInformationProject() {
    let projectid = this.activatedRoute.snapshot.params.id;
    if (this.getStartDate() !== this.f.startDate.value && this.getEndDate !== this.f.endDate.value) {
      this.f.startDate.value.setTime(new Date(new Date(this.f.startDate.value.getTime() - (this.f.startDate.value.getTimezoneOffset() * 60 * 1000)).toUTCString()));
      this.f.endDate.value.setTime(new Date(new Date(this.f.endDate.value.getTime() - (this.f.endDate.value.getTimezoneOffset() * 60 * 1000)).toUTCString()));
      this.projectService.updateProjectInformation(projectid, this.f.projectCode.value.trim(), this.f.projectName.value.trim(), this.f.description.value.trim(), this.f.startDate.value, this.f.endDate.value).subscribe((res: any) => {
        this.project = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update project information successfully.' });
        this.getProjectInformation();
      });
    }
    else if (this.getStartDate() !== this.f.startDate.value) {
      this.f.startDate.value.setTime(new Date(new Date(this.f.startDate.value.getTime() - (this.f.startDate.value.getTimezoneOffset() * 60 * 1000)).toUTCString()));
      this.projectService.updateProjectInformation(projectid, this.f.projectCode.value.trim(), this.f.projectName.value.trim(), this.f.description.value.trim(), this.f.startDate.value, this.f.endDate.value).subscribe((res: any) => {
        this.project = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update project information successfully.' });
        this.getProjectInformation();
      });
    }
    else if (this.getEndDate() !== this.f.endDate.value) {
      this.f.endDate.value.setTime(new Date(new Date(this.f.endDate.value.getTime() - (this.f.endDate.value.getTimezoneOffset() * 60 * 1000)).toUTCString()));
      this.projectService.updateProjectInformation(projectid, this.f.projectCode.value.trim(), this.f.projectName.value.trim(), this.f.description.value.trim(), this.f.startDate.value, this.f.endDate.value).subscribe((res: any) => {
        this.project = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update project information successfully.' });
        this.getProjectInformation();
      });
    }
    else {
      this.projectService.updateProjectInformation(projectid, this.f.projectCode.value.trim(), this.f.projectName.value.trim(), this.f.description.value.trim(), this.f.startDate.value, this.f.endDate.value).subscribe((res: any) => {
        this.project = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update project information successfully.' });
        this.getProjectInformation();
      });
    }
  }

  //Save new infor for project
  saveChangesProjectInfo() {
    this.submitted = true;
    if (this.editprojectInformationForm.invalid) {
      return;
    }
    else {
      var startDate = new Date(this.f.startDate.value);
      var dueDate = new Date(this.f.endDate.value);
      if (startDate !== null && dueDate >= startDate) {
        this.updateInformationProject();
      }
      else {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Due date must be greater than start date!' });
        return;
      }
    }
  }

  // Check startdate and duedate
  checkValidateDate() {
    var startDate = new Date(this.f.startDate.value);
    var dueDate = new Date(this.f.endDate.value);
    if (startDate !== null && dueDate >= startDate) {
      return dueDate;
    }
    else {
      return;
    }
  }

  // Convenience getter for easy access to form fields
  get f() { return this.editprojectInformationForm.controls; }

  shortName(text: string) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }
}

