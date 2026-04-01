import { MessageService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-duedate',
  templateUrl: './task-duedate.component.html',
  styleUrls: ['./task-duedate.component.css']
})
export class TaskDuedateComponent implements OnInit {
  @Input() task!: Task;
  dueDate!: Date;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private taskService: TaskService, private messageService: MessageService) { }

  ngOnInit(): void {
    if (this.task.dueDate) {
      this.dueDate = new Date(this.task.dueDate);
    }
  }

  // API update duedate
  onChange(newDueDate) {
    if (this.task.dueDate !== newDueDate) {
      newDueDate.setTime(new Date(new Date(newDueDate.getTime() - (newDueDate.getTimezoneOffset() * 60 * 1000)).toUTCString()));
      this.dueDate = newDueDate;
      var startDate = new Date(this.task.startDate)
      if (this.dueDate >= startDate) {
        this.taskService.updateDueDate(this.task.taskId, this.dueDate).subscribe((res: any) => {
          this.task.dueDate = res.data;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Due date is updated' });
          this.uploadStatus = true;
          this.emitter.emit(this.uploadStatus);
        });
      }
      else
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Due date must be greater than start date!' });
      return;
    }
    else
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Due date is invalid!' });
    return;
  }

}
