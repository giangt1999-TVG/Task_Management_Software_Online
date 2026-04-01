import { Component, OnInit, ViewChild } from '@angular/core';
import { TaskService } from '@app/_services/task.service';
import { PrimeNGConfig } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import { TaskModelComponent } from '@app/project-management/tasks/task-model/task-model.component';

@Component({
  selector: 'app-my-tasks',
  templateUrl: './my-tasks.component.html',
  styleUrls: ['./my-tasks.component.scss'],
  providers: [DialogService]
})
export class MyTasksComponent implements OnInit {

  tasks: any = [];
  loading!: boolean;
  keyWord!: string;
  @ViewChild('dt') table!: Table;

  constructor(private taskService: TaskService,
    private primengConfig: PrimeNGConfig,
    private dialogService: DialogService) { }

  getAllTasks() {
    this.taskService.getTaskFromAllProject().subscribe((res: any) => {
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

  searchTaskNameKeyDown(event) {
    if (event.keyCode === 13 || event.key === "Enter") {
      if (this.keyWord == '' || this.keyWord == null) {
        this.getAllTasks();
      }
      else {
        this.taskService.searchAllUserTask(this.keyWord).subscribe((res: any) => {
          this.tasks = res.data;
        });
      }
    }
  }

  searchTaskNameBtn() {
    if (this.keyWord == null || this.keyWord == '') {
      this.getAllTasks();
    }
    else {
      this.taskService.searchAllUserTask(this.keyWord).subscribe((res: any) => {
        this.tasks = res.data;
      });
    }
  }

  shortName(text: string) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }

  openTaskModelDialog(taskId: number) {
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

}
