import { Task } from '@app/_models/task';
import { TaskService } from '@app/_services/task.service';
import { MessageService } from 'primeng/api';
import { Component, Inject, Input, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-task-detail-rejected',
  templateUrl: './task-detail-rejected.component.html',
  styleUrls: ['./task-detail-rejected.component.css'],
  providers: [MessageService],
})
export class TaskDetailRejectedComponent implements OnInit {

  section: any;
  priority: any;
  label: any;
  condition: any;
  tasks: any = [];
  //create checklist
  checklists: any = [];

  //create subtask
  subtasks: any = [];

  //create dependency
  dependencies: any = [];

  //uploadd file
  uploadedFiles: any = [];
  checkedActive!: boolean;
  checkedComplete!: boolean;
  startDate!: Date;
  dueDate!: Date;
  fullName!: string;

  constructor(private messageService: MessageService, private taskService: TaskService, @Inject(MAT_DIALOG_DATA) public data: { taskId: string }) {
  }

  ngOnInit(): void {
    this.getTaskInfoDetailById();
    // this.getStartDateOfTask();
  }

  // getStartDateOfTask(){
  //   this.startDate = new Date(this.tasks.startDate);
  // }

  getFullNameUserOfTask() {
    if (this.tasks.assignee.fullName != null) {
      this.fullName = this.tasks.assignee.fullName;
    }
    return this.fullName;
  }

  addCheckListElement() {
    this.checklists.push({ value: "" });
  }
  deleteCheckListElement(i) {
    this.checklists.splice(i, 1);
  }
  countCheckListPercent() {
    let count = 0;

    for (var i = 0; i < this.tasks.checklists; i++) {
      count++;
    }
    return count;
  }
  addSubTaskElement() {
    this.subtasks.push({ value: "" });
  }
  deleteSubTaskElement(j) {
    this.subtasks.splice(j, 1);
  }

  countSubTaskPercent() {
    let count = 0;

    for (var j = 0; j < this.subtasks.length; j++) {
      count++;
    }
    return count;
  }

  addDependenciesElement() {
    this.dependencies.push({ value: "" });
  }
  deleteDependenciesElement(k) {
    this.dependencies.splice(k, 1);
  }

  countDependenciesPercent() {
    let count = 0;

    for (var k = 0; k < this.dependencies.length; k++) {
      count++;
    }
    return count;
  }

  onUpload(event) {
    for (let file of event.files) {
      this.uploadedFiles.push(file);
    }
    this.messageService.add({ severity: 'info', summary: 'File Uploaded', detail: '' });
  }

  //API get task detail by id
  getTaskInfoDetailById() {
    // this.taskService.getTaskDetailbyID(this.data.taskId).subscribe((res: any) => {
    //   this.tasks = res.data;
    // });
  }
}




