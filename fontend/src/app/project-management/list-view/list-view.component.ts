import { TaskService } from '@app/_services/task.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Table } from 'primeng/table';
import { PrimeNGConfig } from 'primeng/api';
import { TaskModelComponent } from '../tasks/task-model/task-model.component';
import { DialogService } from 'primeng/dynamicdialog';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '@app/_services/authentication.service';
import { User } from '@app/_models/user';

@Component({
  selector: 'app-list-view',
  templateUrl: './list-view.component.html',
  styleUrls: ['./list-view.component.scss'],
  providers: [DialogService]
})
export class ListViewComponent implements OnInit {

  lists: any[] = [];
  tasks: any = [];
  loading!: boolean;
  currentUser!: User | null;
  @ViewChild('dt') table!: Table;

  constructor(private taskService: TaskService,
    private dialogService: DialogService,
    private primengConfig: PrimeNGConfig,
    private activatedRoute: ActivatedRoute) { }

  getAllTasks() {
    let projectid = this.activatedRoute.snapshot.params.id;
    this.taskService.getAllTask(projectid).subscribe((res: any) => {
      this.tasks = res.data;
    });
  }

  ngOnInit(): void {
    this.loading = true;
    setTimeout(() => {
      this.getAllTasks();
      this.loading = false;
    }, 1000);
    this.primengConfig.ripple = true;
  }

  openDetailTaskDialog(taskId: number) {
    let dialogRef = this.dialogService.open(TaskModelComponent, {
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
    dialogRef.onClose.subscribe(result => {
      this.getAllTasks();
    });
  }

  shortName(text: string) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }
}
