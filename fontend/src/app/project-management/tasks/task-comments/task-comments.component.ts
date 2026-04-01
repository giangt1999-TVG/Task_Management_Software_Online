import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-comments',
  templateUrl: './task-comments.component.html',
  styleUrls: ['./task-comments.component.css']
})
export class TaskCommentsComponent implements OnInit {
  @Input() task!: Task;
  comments: any = [];
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
    
  }

  receiveDataFromChild(data){
    this.emitter.emit(data);
  }
}
