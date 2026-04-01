import { MessageService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { UserService } from './../../../_services/user.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-assignee',
  templateUrl: './task-assignee.component.html',
  styleUrls: ['./task-assignee.component.css']
})
export class TaskAssigneeComponent implements OnInit {
  @Input() task!: Task;
  assignees!: any[];
  currentAssignee!: any;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private userService: UserService, private activatedRoute: ActivatedRoute, private messageService: MessageService) {
  }

  getProjectID() {
    let projectid = this.activatedRoute.snapshot.params.id;
    return projectid;
  }

  ngOnInit(): void {
    this.getUserInProject(this.task.projectId);
  }

  //API get user project
  getUserInProject(projectId){
    this.userService.getUserInProject(projectId).subscribe((res: any) => {
      this.assignees = res.data;
    });
  }

  //API change user of task
  onChangeUser(selectedUser){
    // if(this.task.assignee.userId !== selectedUser.id)
    this.userService.updateUserAssigneeTask(this.task.taskId, selectedUser.id).subscribe((res: any) => {
      this.task.assignee = res.data;
      this.showSuccessMessage();
    });
  }

  // Toast message success
  showSuccessMessage() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Assignee new user is updated!' });
  }
}
