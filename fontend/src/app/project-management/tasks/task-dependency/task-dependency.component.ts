import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Task } from '@app/_models/task';
import { TaskService } from '@app/_services/task.service';
import { ConfirmationService, MessageService } from "primeng/api";
import { DialogService } from 'primeng/dynamicdialog';
import { TaskModelComponent } from '../task-model/task-model.component';

@Component({
  selector: 'app-task-dependency',
  templateUrl: './task-dependency.component.html',
  styleUrls: ['./task-dependency.component.css'],
  providers: [DialogService]
})
export class TaskDependencyComponent implements OnInit {
  @Input() task!: Task;
  @Output() onClosed = new EventEmitter();
  displayNewPopup: boolean = false;
  displayUpdatePopup: boolean = false;
  dependencyTasks!: any[];
  dependencies!: any[];
  selectedTask!: any;
  selectedDependency!: any;
  currentTaskDependencyId!: number;


  constructor(private _taskService: TaskService,
    private activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private dialogService: DialogService) { }

  ngOnInit(): void {
  }

  showNewDialog() {
    this.selectedTask = null;
    this.selectedDependency = null;
    this.getDependencyTasks();
    this.getListDependencies();
    this.displayNewPopup = true;
  }

  closeNewDialog() {
    this.displayNewPopup = false;
  }

  createNewDependency() {
    if (this.selectedTask && this.selectedDependency) {
      this._taskService.createNewDependency(this.selectedTask.taskId, this.selectedDependency.id, parseInt(this.task.taskId)).subscribe((res: any) => {
        var dependencyTask = {
          dependencyId: res.data.taskDependencyId,
          taskDenpendencyId: this.selectedTask.taskId,
          taskName: this.selectedTask.name,
          dependencyName: this.selectedDependency.name,
          createdDate: ""
        }
        this.task.dependencies.push(dependencyTask);
      })
    }

    this.closeNewDialog();
  }

  showUpdateDialog(taskId: number) {
    this.currentTaskDependencyId = taskId;
    this.selectedTask = null;
    this.selectedDependency = null;
    this.getDependencyTasks();
    this.getListDependencies();
    this.displayUpdatePopup = true;
  }

  closeUpdateDialog() {
    this.displayUpdatePopup = false;
  }

  updateDependency() {
    if (this.selectedTask && this.selectedDependency) {
      this._taskService.updateDependency(this.currentTaskDependencyId, this.selectedTask.taskId, this.selectedDependency.id).subscribe((res: any) => {
        var dependencyTask = this.task.dependencies.find(d => d.dependencyId == this.currentTaskDependencyId);
        if (dependencyTask) {
          dependencyTask.taskName = this.selectedTask.name,
          dependencyTask.dependencyName = this.selectedDependency.name
        }
      })
    }

    this.closeUpdateDialog();
  }

  getDependencyTasks() {
    var projectId = this.activatedRoute.snapshot.params.id;
    this._taskService.getAllDenpendencyTask(projectId).subscribe((res: any) => {
      this.dependencyTasks = res.data;
      this.dependencyTasks.splice(this.dependencyTasks.findIndex(c => c.taskId == parseInt(this.task.taskId)) , 1);
    })
  }

  getListDependencies() {
    this._taskService.getAllDenpendencies().subscribe((res: any) => {
      this.dependencies = res.data;
    })
  }

  confirmDelete(event: Event, taskDependencyId: number) {
    event.stopPropagation();
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      key: "deleteDependencyPopup",
      message: "Are you sure that you want to delete it?",
      icon: "pi pi-exclamation-triangle",
      accept: () => {
        var taskDependency = this.task.dependencies.find(c => c.dependencyId == taskDependencyId);
        if (taskDependency) {
          this._taskService.updateDependency(taskDependencyId, null, null, true).subscribe((res: any) => {
            this.task.dependencies.splice(this.task.dependencies.findIndex(c => c.dependencyId === taskDependencyId) , 1);
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
