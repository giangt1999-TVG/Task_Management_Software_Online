import { MyProjectsComponent } from './project-management/my-projects/my-projects.component';
import { ProjectInformationComponent } from './project-management/project-information/project-information.component';
import { CreateNewProjectComponent } from './project-management/create-new-project/create-new-project.component';
import { HomeComponent } from './home/home/home.component';
import { HomeAdminComponent } from './home/home-admin/home-admin.component';
import { SignupComponent } from './authentication/signup/signup.component';
import { LoginComponent } from './authentication/login/login.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { KanbanViewComponent } from './project-management/kanban-view/kanban-view.component';
import { AuthGuard } from './_helpers/auth.guard';
import { LayoutComponent } from './layout/layout/layout.component';
import { Role } from './_models/role';
import { TaskModelComponent } from './project-management/tasks/task-model/task-model.component';
import { ListViewComponent } from './project-management/list-view/list-view.component';
import { AccountListComponent } from './adminScreen/account-list/account-list.component';
import { EditAccountComponent } from './adminScreen/account-list/edit-account/edit-account.component';
import { ImportAccountComponent } from './adminScreen/account-list/import-account/import-account.component';
import { MyTasksComponent } from './task-management/my-tasks/my-tasks.component';
import { TabMenuComponent } from './project-management/project-layout/tab-menu/tab-menu.component';

// Add path and component
const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        redirectTo: 'home-user',
        pathMatch: 'full'
      },
      {
        path: 'home-user',
        component: HomeComponent
      },
      {
        path: 'home-admin',
        component: HomeAdminComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Admin] }
      },
      {
        path: 'project/:id',
        component: TabMenuComponent,
        children: [
          {
            path: 'list-view/:id',
            component: ListViewComponent,
            children: [
              { path: 'task-details', component: TaskModelComponent }
            ]
          },
          {
            path: 'kanban-view/:id',
            component: KanbanViewComponent,
            children: [
              { path: 'task-details', component: TaskModelComponent }
            ]
          },
          {
            path: 'project-information/:id',
            component: ProjectInformationComponent,
            children: [
              {path: 'project-infortmation', component: ProjectInformationComponent}
            ]
          },
        ]
      },
      {
        path: 'create-new-project',
        component: CreateNewProjectComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Teacher] }
      },
      {
        path: 'account-list',
        component: AccountListComponent,
        children: [
          { path: 'edit-account', component: EditAccountComponent },
          { path: 'import-accounts', component: ImportAccountComponent }
        ],
        canActivate: [AuthGuard],
        data: { roles: [Role.Admin] }
      },
      {
        path: 'my-tasks',
        component: MyTasksComponent,
        children: [
          { path: 'task-details', component: TaskModelComponent }
        ]
      },
      {
        path: 'my-projects',
        component: MyProjectsComponent,
        children: [
          { path: 'my-projects', component: MyProjectsComponent }
        ]
      },

    ]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

//Export RoutingComponent
export const RoutingComponent = [LoginComponent, SignupComponent, HomeComponent, KanbanViewComponent, CreateNewProjectComponent, ProjectInformationComponent]
