import { MessageService } from 'primeng/api';
import { TaskService } from './../../../_services/task.service';
import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-title',
  templateUrl: './task-title.component.html',
  styleUrls: ['./task-title.component.css']
})
export class TaskTitleComponent implements OnInit, OnChanges {
  @Input() task!: Task;
  titleControl!: FormControl;

  constructor(private taskService: TaskService, private messageService: MessageService) {
  }

  ngOnChanges(changes: SimpleChanges): void {
  }

  ngOnInit(): void {
    this.titleControl = new FormControl(this.task.name, { updateOn: 'blur', validators: [Validators.required] });
  }

  // Update new task name
  onChange(event: Event) {
    if (this.task.name !== this.titleControl.value) {
      this.taskService.updateTaskName(this.task.taskId, this.titleControl.value).subscribe((res: any) => {
        this.task.name = this.titleControl.value;
        this.showSuccess();
      });
    }
  }

  // Toast message success
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Task name updated!' });
  }

  // Toast message success
  showFailed() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Update task name failed !' });
  }

}
