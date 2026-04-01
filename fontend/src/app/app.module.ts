import { ProjectInformationComponent } from './project-management/project-information/project-information.component';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AvatarModule } from 'ngx-avatar';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
import { ErrorInterceptor } from './_helpers/error.interceptor';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './authentication/login/login.component';
import { HomeComponent } from './home/home/home.component';
import { SidebarComponent } from './layout/navigation/sidebar/sidebar.component';
import { HeaderComponent } from './layout/header/header.component';
import { KanbanViewComponent } from './project-management/kanban-view/kanban-view.component';
import { NavigationComponent } from './layout/navigation/navigation/navigation.component';
import { ResizerComponent } from './layout/navigation/resizer/resizer.component';

// PrimeNG Components
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { DividerModule } from 'primeng/divider';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { RippleModule } from 'primeng/ripple';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FocusTrapModule } from 'primeng/focustrap';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { PickListModule } from 'primeng/picklist';
import { TooltipModule } from 'primeng/tooltip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { LayoutComponent } from './layout/layout/layout.component';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { EditorModule } from 'primeng/editor';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { ListboxModule } from 'primeng/listbox';
import { TabMenuModule } from 'primeng/tabmenu';
import { MenuItem } from 'primeng/api';
import { ColorPickerModule } from 'primeng/colorpicker';
import { DialogModule } from 'primeng/dialog';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';
import { BadgeModule } from 'primeng/badge';
import { RadioButtonModule } from 'primeng/radiobutton';

// Material Components
import { MatDialogModule } from '@angular/material/dialog';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatButtonModule } from '@angular/material/button';
import { HomeAdminComponent } from './home/home-admin/home-admin.component';
import { CreateNewProjectComponent } from './project-management/create-new-project/create-new-project.component';
import { UserProfileComponent } from './layout/header/user-profile/user-profile.component';
import { TaskModelComponent } from './project-management/tasks/task-model/task-model.component';
import { TaskDetailComponent } from './project-management/tasks/task-detail/task-detail.component';
import { TaskDetailRejectedComponent } from './project-management/task-details/task-detail-rejected/task-detail-rejected.component';
import { TaskTypeComponent } from './project-management/tasks/task-type/task-type.component';
import { TaskTitleComponent } from './project-management/tasks/task-title/task-title.component';
import { TaskDescriptionComponent } from './project-management/tasks/task-description/task-description.component';
import { TaskCommentComponent } from './project-management/tasks/task-comment/task-comment.component';
import { TaskCommentsComponent } from './project-management/tasks/task-comments/task-comments.component';
import { TaskSectionComponent } from './project-management/tasks/task-section/task-section.component';
import { TaskStatusComponent } from './project-management/tasks/task-status/task-status.component';
import { TaskPriorityComponent } from './project-management/tasks/task-priority/task-priority.component';
import { TaskAssigneeComponent } from './project-management/tasks/task-assignee/task-assignee.component';
import { TaskMarkCompletedComponent } from './project-management/tasks/task-mark-completed/task-mark-completed.component';
import { TaskChecklistComponent } from './project-management/tasks/task-checklist/task-checklist.component';
import { TaskSubtaskComponent } from './project-management/tasks/task-subtask/task-subtask.component';
import { TaskDependencyComponent } from './project-management/tasks/task-dependency/task-dependency.component';
import { TaskAttachmentComponent } from './project-management/tasks/task-attachment/task-attachment.component';
import { TaskLabelComponent } from './project-management/tasks/task-label/task-label.component';
import { TaskStartdateComponent } from './project-management/tasks/task-startdate/task-startdate.component';
import { TaskDuedateComponent } from './project-management/tasks/task-duedate/task-duedate.component';
import { ListViewComponent } from './project-management/list-view/list-view.component';
import { MatTableModule } from '@angular/material/table';
import { MatListModule } from '@angular/material/list';
import { AccountListComponent } from './adminScreen/account-list/account-list.component';
import { ImportAccountComponent } from './adminScreen/account-list/import-account/import-account.component';
import { EditAccountComponent } from './adminScreen/account-list/edit-account/edit-account.component';
import { MyTasksComponent } from './task-management/my-tasks/my-tasks.component';

import { AngularFireMessagingModule } from '@angular/fire/messaging';
import { AngularFireDatabaseModule } from '@angular/fire/database';
import { AngularFireAuthModule } from '@angular/fire/auth';
import { AngularFireModule } from '@angular/fire';
import { MessagingService } from './_services/messaging.service';
import { environment } from '../environments/environment';
import { AsyncPipe } from '../../node_modules/@angular/common';
import { AddMemberComponent } from './project-management/project-information/add-member/add-member.component';
import { TabMenuComponent } from './project-management/project-layout/tab-menu/tab-menu.component';
import { NgPipesModule } from 'ngx-pipes';
import { LabelManagementComponent } from './project-management/project-information/label-management/label-management.component';
import { MyProjectsComponent } from './project-management/my-projects/my-projects.component';
import { AddAccountComponent } from './adminScreen/account-list/add-account/add-account.component';
import { UpdateTeacherComponent } from './project-management/project-information/update-teacher/update-teacher.component';
import { UpdateTeamleadComponent } from './project-management/project-information/update-teamlead/update-teamlead.component';
import { NewProjectDialogComponent } from './project-management/my-projects/new-project-dialog/new-project-dialog.component';
import { ChangePasswordComponent } from './layout/header/change-password/change-password.component';
@NgModule({
  declarations: [
    AppComponent,
    KanbanViewComponent,
    HomeComponent,
    LoginComponent,
    NavigationComponent,
    ResizerComponent,
    SidebarComponent,
    HeaderComponent,
    LayoutComponent,
    HomeAdminComponent,
    CreateNewProjectComponent,
    UserProfileComponent,
    ListViewComponent,
    AccountListComponent,
    ImportAccountComponent,
    EditAccountComponent,
    TaskModelComponent,
    TaskDetailComponent,
    TaskDetailRejectedComponent,
    TaskTypeComponent,
    TaskTitleComponent,
    TaskDescriptionComponent,
    TaskCommentComponent,
    TaskCommentsComponent,
    TaskSectionComponent,
    TaskStatusComponent,
    TaskPriorityComponent,
    TaskAssigneeComponent,
    TaskMarkCompletedComponent,
    TaskChecklistComponent,
    TaskSubtaskComponent,
    TaskDependencyComponent,
    TaskAttachmentComponent,
    TaskLabelComponent,
    TaskStartdateComponent,
    TaskDuedateComponent,
    ListViewComponent,
    ProjectInformationComponent,
    AddMemberComponent,
    MyTasksComponent,
    TabMenuComponent,
    LabelManagementComponent,
    MyProjectsComponent,
    AddAccountComponent,
    UpdateTeacherComponent,
    UpdateTeamleadComponent,
    NewProjectDialogComponent,
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    PasswordModule,
    DividerModule,
    ToggleButtonModule,
    RippleModule,
    MatCardModule,
    MatDialogModule,
    CalendarModule,
    DropdownModule,
    CheckboxModule,
    FileUploadModule,
    InputTextareaModule,
    FormsModule,
    FocusTrapModule,
    MessagesModule,
    MessageModule,
    AvatarModule,
    AvatarGroupModule,
    TooltipModule,
    OverlayPanelModule,
    PickListModule,
    DragDropModule,
    ToastModule,
    MultiSelectModule,
    MatGridListModule,
    MatMenuModule,
    MatIconModule,
    ScrollingModule,
    MatButtonModule,
    ToastModule,
    DynamicDialogModule,
    EditorModule,
    TagModule,
    TableModule,
    MatTableModule,
    MatListModule,
    TabMenuModule,
    ConfirmDialogModule,
    ListboxModule,
    ColorPickerModule,
    DialogModule,
    ConfirmPopupModule,
    NgPipesModule,
    BadgeModule,
    RadioButtonModule,
    AngularFireDatabaseModule,
    AngularFireAuthModule,
    AngularFireMessagingModule,
    AngularFireModule.initializeApp(environment.firebase)
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    MessageService,
    MessagingService,
    ConfirmationService,
    AsyncPipe
  ],
  bootstrap: [AppComponent],
  entryComponents: [TaskModelComponent, ChangePasswordComponent]
})
export class AppModule { }
