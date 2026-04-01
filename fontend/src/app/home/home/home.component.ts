import { NotificationService } from './../../_services/notification.service';
import { DialogService } from 'primeng/dynamicdialog';
import { User } from './../../_models/user';
import { TaskService } from './../../_services/task.service';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { ProjectSerVice } from '@app/_services/project.service';
import { AuthenticationService } from '@app/_services/authentication.service';
import { TaskModelComponent } from '@app/project-management/tasks/task-model/task-model.component';
import { MessagingService } from '@app/_services/messaging.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [DialogService]
})

export class HomeComponent implements OnInit {

  shortName!: string | undefined;
  currentUser!: User | null;
  tasks: any[] = [];
  projects: any[] = [];
  isShowDivTask = false;
  isShowDivProject = false;
  keyWord: any;
  results!: string[];
  filteredProjects: any[] = [];
  projectsSearch!: any[];
  loading!: boolean;
  today = new Date();
  currentHours = this.today.getHours();
  currentTime!: string;


  constructor(private router: Router, 
    private projectService: ProjectSerVice, 
    private taskService: TaskService, 
    private author: AuthenticationService, 
    private dialogService: DialogService) {
    this.author.currentUser.subscribe(x => this.currentUser = x);
  }

  // API get my project
  getMyProject() {
    this.projectService.getAllProject().subscribe((res: any) => {
      this.projects = res.data;
    });
  }

  // API get task due soon
  getTaskDue() {
    this.taskService.getTaskDueSoon().subscribe((res: any) => {
      this.tasks = res.data;
    });
  }

  // API get projects for search bar
  getProjectForSearchBar(event) {
    if (event.keyCode === 13 || event.key === "Enter") {
      if (this.keyWord == '' || this.keyWord == null) {
        this.getMyProject();
      }
      else {
        this.projectService.searchProjectByCode(this.keyWord).subscribe((res: any) => {
          this.projects = res.data;
        });
      }
    }
  }

  ngOnInit() {
    this.getMyProject();
    this.getTaskDue();
    this.shortName = this.getShortName(this.currentUser?.fullName);
    if (this.currentHours < 12) {
      this.currentTime = "Good morning";
    } else if (this.currentHours < 18) {
      this.currentTime = "Good afternoon";
    } else {
      this.currentTime = "Good evening";
    }
  }

  //Navigate to create new project
  goToCreateNewProject() {
    this.router.navigateByUrl("/create-new-project");
  }

  private getShortName(text) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }

  toggleDisplayTaskDueSoon() {
    this.isShowDivTask = !this.isShowDivTask;
  }

  toggleDisplayProject() {
    this.isShowDivProject = !this.isShowDivProject;
  }

  //Open dialog task details
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
      this.getTaskDue();
  });
  }
}
