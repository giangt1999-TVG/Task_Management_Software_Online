import { MessageService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { OnChanges, SimpleChanges } from '@angular/core';
import { Component, OnInit, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-description',
  templateUrl: './task-description.component.html',
  styleUrls: ['./task-description.component.css']
})
export class TaskDescriptionComponent implements OnInit, OnChanges {
  @Input() task!: Task;
  descriptionControl!: FormControl;
  textHTML!: string | null;
  isEditing!: boolean;
  isWorking!: boolean;

  constructor(private taskService: TaskService, private messageService: MessageService) { 
  }

  ngOnChanges(changes: SimpleChanges): void {
  }

  ngOnInit(): void {
    this.descriptionControl = new FormControl(this.task.description);
    this.textHTML = this.descriptionControl.value;
  }

  setEditMode(mode: boolean) {
    this.isEditing = mode;
  }


  save() {
    this.setEditMode(false);
    this.taskService.updateTaskDescription(this.task.taskId, this.descriptionControl.value).subscribe((res: any) => {
      this.textHTML = this.descriptionControl.value;
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Task description updated!' });
    });
  }

  cancel() {
    this.setEditMode(false);
  }
}
