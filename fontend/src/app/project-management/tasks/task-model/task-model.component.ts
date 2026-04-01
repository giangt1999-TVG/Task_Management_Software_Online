import { Component, OnInit, Input } from '@angular/core';
import { DeleteTaskModel } from '@app/_models/delete-task-model';
import { Task } from '@app/_models/task';
import { TaskService } from '@app/_services/task.service';
import { Observable } from 'rxjs';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-task-model',
  templateUrl: './task-model.component.html',
  styleUrls: ['./task-model.component.css']
})
export class TaskModelComponent implements OnInit {
  task!: Task;

  constructor(
    private _modal: DynamicDialogRef,
    private _taskService: TaskService,
    private _config: DynamicDialogConfig,
    private messageService: MessageService
  ) {
  }

  ngOnInit(): void {
    this.getTaskDetailInfo(this._config.data.taskId);
  }

  getTaskDetailInfo(taskId: number) {
    this._taskService.getTaskDetailbyID(taskId).subscribe((res: any) => {
      this.task = res.data;
    },
      (err: any) => {
        if (err.status === 400) {
          this.closeModal();
          this.messageService.add({ key: 'error-message', severity: 'error', summary: 'Error', detail: err.error });
        }
      });
  }

  closeModal() {
    this._modal.close();
  }

  receiveDataFromChild(event) {
    this.getTaskDetailInfo(this._config.data.taskId);
  }

  // TODO: Handle logic for deleting task
  // deleteTask({ taskId, deleteModalRef }: DeleteTaskModel) {
  //   this._taskService.deleteIssue(taskId);
  //   deleteModalRef.close();
  //   this.closeModal();
  // }
}
