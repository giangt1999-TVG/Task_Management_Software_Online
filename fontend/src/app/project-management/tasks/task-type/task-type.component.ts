import { Component, OnInit, Input } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-type',
  templateUrl: './task-type.component.html',
  styleUrls: ['./task-type.component.css']
})
export class TaskTypeComponent implements OnInit {
  @Input() task!: Task;
  icon!: string;
  taskType!: string;


  constructor() { 
  }

  ngOnInit(): void {
    if (this.task.parentId === 0 || this.task.parentId === null) {
      this.icon = "check-square";
      this.taskType = "TASK";
    } else {
      this.icon = "share-alt-square";
      this.taskType = "SUBTASK";
    }
  }
}
