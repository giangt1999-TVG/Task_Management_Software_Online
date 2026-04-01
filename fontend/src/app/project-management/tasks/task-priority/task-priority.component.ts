import { MessageService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-priority',
  templateUrl: './task-priority.component.html',
  styleUrls: ['./task-priority.component.css']
})
export class TaskPriorityComponent implements OnInit {
  @Input() task!: Task;
  priorities!: any[];
  currentPriority!: any;
  currentPriorityValue: any;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private taskService: TaskService, private messageService: MessageService) {
    this.getAllPrioritiesOfTask();
  }

  ngOnInit(): void {
  }

  // API get priorities of task
  getAllPrioritiesOfTask() {
    this.taskService.getAllTaskPriorities().subscribe((res: any) => {
      this.priorities = res.data;
    });
  }

  // API change section of task
  onChangePriority(selectedPriority) {
    if(selectedPriority.id !== this.task.priority.priorityId){
      this.taskService.updateTaskPriority(this.task.taskId, selectedPriority.id).subscribe((res: any) => {
        this.task.priority = res.data;
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
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Priority of task is updated!' });
  }

}
