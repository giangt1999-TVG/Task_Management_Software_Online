import { MessageService, ConfirmationService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { Task } from '@app/_models/task';

@Component({
  selector: 'app-task-attachment',
  templateUrl: './task-attachment.component.html',
  styleUrls: ['./task-attachment.component.css']
})
export class TaskAttachmentComponent implements OnInit {
  @Input() task!: Task;
  uploadNewFile!: File;
  fileUpload!: string;
  fileAttachments!: any[];
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;
  constructor(private taskService: TaskService, private messageService: MessageService, private confirmationService: ConfirmationService) { }


  ngOnInit(): void {
  }

  //Select File to upload
  selectedFileUpload(event) {
    if (event.target.files) {
      this.uploadNewFile = event.target.files[0];
    }
    this.taskService.uploadFile(this.task.taskId, this.uploadNewFile).subscribe((res: any) => {
      this.fileUpload = res.data.fileAttachments;
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Upload file successfully' });
      this.uploadStatus = true;
      this.emitter.emit(this.uploadStatus);
    }
    );
  }

  // Delete a upload file 
  deleteFile(fileId: number) {
    this.confirmationService.confirm({
      message: "Do you want to delete this file?",
      header: "Delete Confirmation",
      icon: "pi pi-info-circle",
      accept: () => {
        this.taskService.deleteFileInTask(fileId, this.task.taskId).subscribe((res: any) => {
          this.fileAttachments = res.data;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File is deleted!' });
          this.uploadStatus = true;
          this.emitter.emit(this.uploadStatus);
        });
      },
      reject: () => {
        return;
      }
    })
  }

}
