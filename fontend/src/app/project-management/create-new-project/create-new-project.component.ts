import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ProjectSerVice } from '@app/_services/project.service';
import { Teacher } from '@app/_models/teacher';
import { UserService } from './../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Student } from '@app/_models/student';

@Component({
  selector: 'app-create-new-project',
  templateUrl: './create-new-project.component.html',
  styleUrls: ['./create-new-project.component.css']
})
export class CreateNewProjectComponent implements OnInit {

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
  studentSelectedId: any[] = [];

  constructor(private formBuilder: FormBuilder, private userService: UserService, private projectService: ProjectSerVice, private messageService: MessageService, private router: Router) {
  }

  ngOnInit(): void {
    this.projectInformationForm = this.formBuilder.group({
      projectCode: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(450), Validators.pattern(/([a-zA-Z0-9])$/)]],
      projectName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(250), Validators.pattern(/([a-zA-Z0-9])$/)]],
      durationFrom: ['', [Validators.required]],
      durationTo: ['', [Validators.required]],
      teacherSelected: ['', [Validators.required]],
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
    this.projectService.createNewProject(this.f.projectCode.value.trim(), this.f.projectName.value.trim(), this.f.description.value.trim(),
      this.f.durationFrom.value, this.f.durationTo.value, this.getTeacherSelectedId(),
      this.getTeamLeadSelectedId(), this.studentSelectedId).subscribe((res: any) => {
        this.projects = res.data;
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
    return this.studentSelectedId;
  }

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
      if (startDate !== null && dueDate >= startDate) {
        this.projectInformationForm.value;
        this.createNewProjectAPI();
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Create a new project information successfully' });
        this.router.navigate(["/my-projects"]);
      }
      else {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Due date must be greater than start date!' });
        return;
      }
    }
  }

}
