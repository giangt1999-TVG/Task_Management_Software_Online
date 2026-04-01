import { MessageService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-status',
  templateUrl: './task-status.component.html',
  styleUrls: ['./task-status.component.css']
})
export class TaskStatusComponent implements OnInit {
  @Input() task!: Task;
  statuses!: any[];
  currentStatus!: any;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private taskService: TaskService, private messageService: MessageService) {
    this.getAllStatusOfTask();
  }

  ngOnInit(): void {
  }

  // API get status of task
  getAllStatusOfTask() {
    this.taskService.getAllTaskStatus().subscribe((res: any) => {
      this.statuses = res.data;
    });
  }

  // Update status of task
  onChangeStatus(selectedStatus) {
    if (selectedStatus.id !== this.task.status.statusId) {
      this.taskService.updateTaskStatus(this.task.taskId, selectedStatus.id).subscribe((res: any) => {
        this.task.status = res.data;
        this.showSuccessMessage();
        this.uploadStatus = true;
        this.emitter.emit(this.uploadStatus);
      });
    }
    else
      return;
  }

  // Toast message success
  showSuccessMessage() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Status of task is updated!' });
  }

}
