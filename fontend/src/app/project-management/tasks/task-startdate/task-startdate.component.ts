import { MessageService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-startdate',
  templateUrl: './task-startdate.component.html',
  styleUrls: ['./task-startdate.component.css']
})
export class TaskStartdateComponent implements OnInit {
  @Input() task!: Task;
  startDate!: Date;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private taskService: TaskService, private messageService: MessageService) { }

  ngOnInit(): void {
    if (this.task.startDate) {
      this.startDate = new Date(this.task.startDate);
    }
  }

  // API update start date
  onChange(newStartDate) {
    if (this.task.startDate !== newStartDate) {
      newStartDate.setTime(new Date(new Date(newStartDate.getTime() - (newStartDate.getTimezoneOffset() * 60 * 1000)).toUTCString()));
      this.startDate = newStartDate;
      var dueDate = new Date(this.task.dueDate);
      if (this.task.dueDate === null) {
        this.taskService.updateStartDate(this.task.taskId, this.startDate).subscribe((res: any) => {
          this.task.startDate = res.data;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Start Date is updated!' });
          this.uploadStatus = true;
          this.emitter.emit(this.uploadStatus);
        });
      }
      else if (this.startDate <= dueDate && this.task.dueDate !== null) {
        this.taskService.updateStartDate(this.task.taskId, this.startDate).subscribe((res: any) => {
          this.task.startDate = res.data;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Start Date is updated!' });
          this.uploadStatus = true;
          this.emitter.emit(this.uploadStatus);
        });
      }
      else
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Start date must be smaller than due date!' });
      return;
    }
    else
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Start Date is invalid!' });
    return;
  }
}
