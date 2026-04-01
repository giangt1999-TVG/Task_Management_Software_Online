import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subtask, Task } from '@app/_models/task';
import { TaskService } from '@app/_services/task.service';
import { ConfirmationService, MessageService } from "primeng/api";
import { DialogService } from 'primeng/dynamicdialog';
import { TaskModelComponent } from '../task-model/task-model.component';

@Component({
  selector: 'app-task-subtask',
  templateUrl: './task-subtask.component.html',
  styleUrls: ['./task-subtask.component.css'],
  providers: [DialogService]
})
export class TaskSubtaskComponent implements OnInit {
  @Input() task!: Task;
  @Output() onClosed = new EventEmitter();
  displayNewPopup: boolean = false;
  displayUpdatePopup: boolean = false;
  subtasks!: Subtask[];
  selectedSubtask!: Subtask;
  currentSubtask!: Subtask;
  currentSubtaskId!: number;

  constructor(
    private _taskService: TaskService,
    private activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private dialogService: DialogService) { }

  ngOnInit(): void {
  }

  closeNewDialog() {
    this.displayNewPopup = false;
  }

  showNewDialog() {
    this.getSubtaskOptions();
    this.displayNewPopup = true;
  }

  async showUpdateDialog(taskId: number) {
    this.currentSubtaskId = taskId;
    this.getSubtaskOptions();
    this.displayUpdatePopup = true;
  }

  closeUpdateDialog() {
    this.displayUpdatePopup = false;
  }

  updateSubtask() {
    if (this.currentSubtask) {
      this._taskService.updateSubtask(this.currentSubtaskId, this.currentSubtask.taskId, parseInt(this.task.taskId)).subscribe((res: any) => {
        var subtask = this.task.subtasks.find(s => s.taskId == this.currentSubtaskId);
        if (subtask) {
          subtask.taskId = this.currentSubtask.taskId;
          subtask.name = this.currentSubtask.name;
        }
      });
    }

    this.closeUpdateDialog();
  }

  createNewSubtask() {
    if (this.selectedSubtask) {
      this._taskService.createNewSubtask(this.selectedSubtask.taskId, parseInt(this.task.taskId)).subscribe((res: any) => {
        var subtask = res.data;
        this.task.subtasks.push(subtask);
      });
    }
    this.closeNewDialog();
  }

  getSubtaskOptions() {
    var projectId = this.activatedRoute.snapshot.params.id;
    this._taskService.getAllSubtask(projectId).subscribe((res: any) => {
      this.subtasks = res.data;
      this.subtasks.splice(this.subtasks.findIndex(c => c.taskId == parseInt(this.task.taskId)) , 1);
    });
  }

  confirmDelete(event: Event, taskId: number) {
    event.stopPropagation();
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      key: "deleteSubtaskPopup",
      message: "Are you sure that you want to delete it?",
      icon: "pi pi-exclamation-triangle",
      accept: () => {
        var subtask = this.task.subtasks.find(c => c.taskId == taskId);
        if (subtask) {
          this._taskService.deleteSubtask(taskId).subscribe((res: any) => {
            this.task.subtasks.splice(this.task.subtasks.findIndex(c => c.taskId === taskId) , 1);
          });

          this.messageService.add({
            severity: "info",
            summary: "Delete successfully!"
          });
        }
      },
      reject: () => {
      }
    });
  }

  //Open dialog task details
  openTaskModelDialog(event: Event, taskId: number) {
    event.stopPropagation();
    this.onClosed.emit();
    this.dialogService.open(TaskModelComponent, {
      data: {
        taskId: taskId
      },
      showHeader: false,
      width: '1050px',
      contentStyle: {
        'padding': '15px 25px'
      },
      style: {
        'margin-left': '3%',
        'margin-right': '3%'
      },
      dismissableMask: false
    });
  }

}
