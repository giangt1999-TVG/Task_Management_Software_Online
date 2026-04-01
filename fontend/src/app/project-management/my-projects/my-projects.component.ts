import { ProjectSerVice } from '@app/_services/project.service';
import { AuthenticationService } from '@app/_services/authentication.service';
import { User } from '@app/_models/user';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewProjectDialogComponent } from './new-project-dialog/new-project-dialog.component';

@Component({
  selector: 'app-my-projects',
  templateUrl: './my-projects.component.html',
  styleUrls: ['./my-projects.component.scss']
})
export class MyProjectsComponent implements OnInit {

  isShowDivProject = false;
  projects: any = [];
  keyWord: any;
  selectedValue: any;
  currentUser!: User | null;
  listProjectCode: any = [];

  constructor(private router: Router, private author: AuthenticationService, private matDialog: MatDialog, private projectService: ProjectSerVice) {
    this.author.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    this.getMyProject();
  }

  // API get my project
  getMyProject() {
    this.projectService.getAllProject().subscribe((res: any) => {
      this.projects = res.data;
    });
  }

  //Get list project code
  getListCodeProject() {
    for (let index = 0; index < this.projects.length; index++) {
      this.listProjectCode.push(this.projects[index].code);
    }
    return this.listProjectCode;
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

  toggleDisplayProject() {
    this.isShowDivProject = !this.isShowDivProject;
  }

  //Navigate to create new project
  goToCreateNewProject() {
    this.router.navigateByUrl("/create-new-project");
  }

  shortName(text: string) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }

  openNewProjectDialog() {
    let dialogRef = this.matDialog.open(NewProjectDialogComponent, { data: { listProjectCode: this.getListCodeProject(), userId: this.currentUser?.id }, autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
    });
    this.getMyProject();
  }

}
