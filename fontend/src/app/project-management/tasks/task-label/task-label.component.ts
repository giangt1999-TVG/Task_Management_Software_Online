import { MessageService } from 'primeng/api';
import { Label } from './../../../_models/task';
import { TaskService } from '@app/_services/task.service';
import { ActivatedRoute } from '@angular/router';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-label',
  templateUrl: './task-label.component.html',
  styleUrls: ['./task-label.component.css']
})
export class TaskLabelComponent implements OnInit {
  @Input() task!: Task;
  labelCurrent: any;
  newLabel: any;
  labels!: any[];
  selectedLabel: any;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private activatedRoute: ActivatedRoute, private taskService: TaskService, private messageService: MessageService) {
  }

  ngOnInit(): void {
    this.labelCurrent = this.task.labels[0];
    this.getAllProjectLabel(this.task.projectId);
  }

  getProjectID() {
    let projectid = this.activatedRoute.snapshot.params.id;
    return projectid;
  }

  //Get all label in project
  getAllProjectLabel(projectId) {
    this.taskService.getAllLabel(projectId).subscribe((res: any) => {
      this.labels = res.data;
    });
  }

  // Change label of task
  onChangeLabel(selectedLabel) {
    if (this.task.labels.length === 0) {
      this.taskService.updateLabel(this.task.taskId, selectedLabel.labelID).subscribe((res: any) => {
        var newLabel = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Label of task is updated!' });
        this.uploadStatus = true;
        this.emitter.emit(this.uploadStatus);
        // this.newLabel = this.task.labels.find(l => l.labelId == newLabel.labelId);
        this.labelCurrent = this.labels.find(l => {
          return l.labelID == selectedLabel.labelID;
        });
      });
    }
    else if (selectedLabel.labelID !== this.labelCurrent.labelId) {
      this.taskService.updateLabel(this.task.taskId, selectedLabel.labelID).subscribe((res: any) => {
        this.labelCurrent = this.labels.find(l => {
          return l.labelID == selectedLabel.labelID;
        });
        var newLabel = res.data;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Label of task is updated!' });
        this.uploadStatus = true;
        this.emitter.emit(this.uploadStatus);
        // this.newLabel = this.task.labels.find(l => l.labelId == newLabel.labelId);
      });
    }
    else
      return;
  }
}
