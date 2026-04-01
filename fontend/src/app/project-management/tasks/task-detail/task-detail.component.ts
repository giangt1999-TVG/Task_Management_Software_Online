import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { TaskAttachmentComponent } from './../task-attachment/task-attachment.component';
import { ConfirmationService, MessageService, PrimeNGConfig } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { EventEmitter, ViewChild } from '@angular/core';
import { Component, OnInit, Input, Output } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.css'],
  providers: [ConfirmationService]
})
export class TaskDetailComponent implements OnInit {
  @Input() task!: Task;
  @Output() onClosed = new EventEmitter();
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  projectID: any;
  constructor(private taskService: TaskService,
     private messageService: MessageService,
      private confirmationService: ConfirmationService,
       private primengConfig: PrimeNGConfig,
       private _config: DynamicDialogConfig,
       ) { }

  ngOnInit(): void {
    this.primengConfig.ripple = true;    
  }

  closeModal() {
    this.onClosed.emit();
  }

  // Delete task
  deleteTask() {
    this.confirmationService.confirm({
      message: "Do you want to delete this task?",
      header: "Delete Confirmation",
      icon: "pi pi-info-circle",
      accept: () => {
        this.taskService.deleteTask(this.task.taskId).subscribe((res: any) => {          
          this.task = res.data;
          this.closeModal();
          this.showSuccess();
        });
      },
      reject: () => {
        return;
      }
    })
  }

  receiveDataFromChild(data){
    this.emitter.emit(data);
  }

  // Toast message success
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Task is deleted!' });
  }
}
