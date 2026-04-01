import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ProjectSerVice } from '@app/_services/project.service';
import { Teacher } from '@app/_models/teacher';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Student } from '@app/_models/student';
import { CompileShallowModuleMetadata } from '@angular/compiler';
import { UserService } from '@app/_services/user.service';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-new-project-dialog',
  templateUrl: './new-project-dialog.component.html',
  styleUrls: ['./new-project-dialog.component.scss']
})
export class NewProjectDialogComponent implements OnInit {

  projectInformationForm!: FormGroup;
  projects: any[] = [];
  submitted = false;
  projectCode!: string;
  projectName!: string;
  durationFrom!: Date;
  durationTo!: Date;
  description!: string;
  students: any[] = [];
  teachers: any[] = [];
  studentsIsNotTeamLead: any[] = [];
  studentSelected: Student[] = [];
  teamleadSelected: any;
  teacherSelected: any;
  teamLeadSelectedId!: string;
  teacherSelectedId!: string;
  studentSelectedId: any = [];
  listProjectCode: any = [];
  projectChecked!: any;
  teacherID!: any;

  constructor(private formBuilder: FormBuilder,
    private userService: UserService,
    private projectService: ProjectSerVice,
    private messageService: MessageService,
    private router: Router,
    private matDialog: MatDialog, @Inject(MAT_DIALOG_DATA) data) {
    this.listProjectCode = data.listProjectCode;
    this.teacherID = data.userId;
  }

  ngOnInit(): void {
    this.projectInformationForm = this.formBuilder.group({
      projectCode: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150), Validators.pattern(/([a-zA-Z0-9])$/)]],
      projectName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(250), Validators.pattern(/([a-zA-Z0-9])$/)]],
      durationFrom: ['', [Validators.required]],
      durationTo: ['', [Validators.required]],
      teacherSelected: this.teacherID,
      teamleadSelected: ['', [Validators.required]],
      studentSelected: [],
      description: ['', [Validators.minLength(0), Validators.maxLength(1000)]],
    });
    this.getAllStudent();
    this.getAllTeacher();
  }

  // API get all student
  getAllStudent() {
    this.userService.getAllStudent().subscribe((res: any) => {
      this.students = res.data;
    });
  }

  // API get all teacher
  getAllTeacher() {
    this.userService.getAllTeacher().subscribe((res: any) => {
      this.teachers = res.data;
    });
  }

  // API post create new project
  createNewProjectAPI() {
    this.getStudentSelectedId();
    this.projectService.createNewProject(this.f.projectCode.value, this.f.projectName.value, this.f.description.value,
      this.f.durationFrom.value, this.f.durationTo.value, this.teacherID,this.getTeamLeadSelectedId(), this.studentSelectedId).subscribe((res: any) => {
        this.projects = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Create a new project information successfully' });
        this.matDialog.closeAll();
        this.router.navigate(['/project', res.data.id, 'kanban-view', res.data.id]);
      },
        (error) => {
          this.messageService.add({ key: 'tr', severity: 'error', summary: 'Error', detail: error.error });
          return;
        });
  }

  // Check startdate and duedate
  checkValidateDate() {
    var startDate = new Date(this.f.durationFrom.value);
    var dueDate = new Date(this.f.durationTo.value);
    if (startDate !== null && dueDate >= startDate) {
      return dueDate;
    }
    else {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Due date must be greater than start date!' });
      return;
    }
  }

  // Get id of teacher selected
  getTeacherSelectedId() {
    if (this.teacherSelected !== null) {
      this.teacherSelectedId = this.f.teacherSelected.value[0].id;
    }
    return this.teacherSelectedId;
  }

  // Get id of teamlead selected
  getTeamLeadSelectedId() {
    if (this.teamleadSelected !== null) {
      this.teamLeadSelectedId = this.f.teamleadSelected.value[0].id;
    }
    return this.teamLeadSelectedId;
  }

  // Get list id of student selected
  getStudentSelectedId() {
    for (let index = 0; index < this.f.studentSelected.value.length; index++) {
      if (this.f.studentSelected.value[index].id !== this.getTeamLeadSelectedId()) {
        this.studentSelectedId.push(this.f.studentSelected.value[index].id);
      }
    }
    console.log(this.f.studentSelected.value);
    console.log(this.studentSelectedId);
  }


  //Check exist code
  // checkExsistCodeProject() {
  // for (let index = 0; index < this.listProjectCode.length; index++) {
  //   if (this.f.projectCode.value.trim() === this.listProjectCode[index]) {
  //     this.projectChecked = null;
  //   }
  //   else {
  //     this.projectChecked = this.f.projectCode.value.trim();
  //   }
  // }
  // this.listProjectCode.map(o => {
  //   if (this.f.projectCode.value !== o) {
  //     this.projectChecked = o;
  //   }
  //   return this.projectChecked;
  // })
  // }

  //   this.project.listUser.map(o => {
  //     if (this.currentUser?.id === o.userId) {
  //         this.userRole = o.role;
  //     }
  // })

  // Convenience getter for easy access to form fields
  get f() { return this.projectInformationForm.controls; }

  createNewProject() {
    this.submitted = true;
    if (this.projectInformationForm.invalid) {
      return;
    }
    else {
      var startDate = new Date(this.f.durationFrom.value);
      var dueDate = new Date(this.f.durationTo.value);
      // this.checkExsistCodeProject();
      // if (this.checkExsistCodeProject() !== null) {
      if (startDate !== null && dueDate >= startDate) {
        this.createNewProjectAPI();
      }
      else {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Due date must be greater than start date!' });
        return;
      }
    }
  }
}
