import { TaskList } from './../_models/task-list';
import { List } from './../_models/list';
import { Checklist, Task } from '@app/_models/task';
import { ProjectSerVice } from '@app/_services/project.service';
import { AuthenticationService } from './authentication.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';


@Injectable({
  providedIn: 'root'
})

export class TaskService {

  constructor(private http: HttpClient, private project: ProjectSerVice, private author: AuthenticationService) {
  }
  // API get All List and Task
  getAllTask(projectID: string): Observable<TaskList[]> {
    return this.http.get<TaskList[]>(`${environment.apiUrl}/api/task/listview?projectId=` + projectID);
  }

  // API get task due soon
  getTaskDueSoon(): Observable<Task[]> {
    var daysDueIn = 3;
    return this.http.get<Task[]>(`${environment.apiUrl}/api/task/due-soon-task?userId=` + this.author.currentUserValue?.id + `&daysDueIn=` + daysDueIn);
  }

  // API create new task in List
  createNewTask(ListId: string, taskName: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { listId: ListId, name: taskName };
    const body = JSON.stringify(obj);
    return this.http.post<any>(`${environment.apiUrl}/api/task/create`, body, { 'headers': headers });
  }

  // API update position task
  updateTaskPosition(TaskId: string, ListId: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: TaskId,
      listId: ListId
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/task-position`, body, { 'headers': headers });
  }

  // Get task detail by id
  getTaskDetailbyID(taskID: number): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/api/task/detail/` + taskID);
  }

  // Get all status of task
  getAllTaskStatus(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/status`);
  }

  // Get all priorities of task 
  getAllTaskPriorities(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/priority`);
  }

  // API  Update DueDate for task
  updateDueDate(TaskId: string, dueDate: Date): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: TaskId,
      dueDate: dueDate
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/duedate`, body, { 'headers': headers });
  }

  // API  Update StartDate for task
  updateStartDate(TaskId: string, startDate: Date): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: TaskId,
      startDate: startDate
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/startdate`, body, { 'headers': headers });
  }

  // API delete task
  deleteTask(taskId: string): Observable<Task[]> {
    return this.http.delete<Task[]>(`${environment.apiUrl}/api/task/` + taskId);
  }


  // API update task name
  updateTaskName(TaskId: string, name: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: TaskId,
      name: name
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/name`, body, { 'headers': headers });
  }

  //API description for task
  updateTaskDescription(TaskId: string, description: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: TaskId,
      description: description
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/description`, body, { 'headers': headers });
  }

  // API Update priority for task
  updateTaskPriority(taskId: string, priorityId: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: taskId,
      priorityId: priorityId
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/priority`, body, { 'headers': headers });
  }

  // API Create a new comment
  createNewComment(userId: string, taskId: string, content: string, attachFile: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { userId: userId, taskId: taskId, content: content, attachFile: attachFile };
    const body = JSON.stringify(obj);
    return this.http.post<any>(`${environment.apiUrl}/api/task/comment`, body, { 'headers': headers });
  }

  // Api new subtask
  createNewSubtask(subtaskId: number, taskId: number): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { subtaskId: subtaskId, taskId: taskId };
    const body = JSON.stringify(obj);
    return this.http.post<any>(`${environment.apiUrl}/api/task/subtask`, body, { 'headers': headers });
  }

  // Api delete subtask
  deleteSubtask(taskId: number): Observable<any> {
    return this.http.delete<any>(`${environment.apiUrl}/api/task/subtask?id=` + taskId);
  }

  // Api create new checklist
  createNewChecklist(name: string, taskId: number): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { name: name, isCompleted: false, taskId: taskId };
    const body = JSON.stringify(obj);
    return this.http.post<any>(`${environment.apiUrl}/api/task/checklist`, body, { 'headers': headers });
  }

  // Api update checklist
  updateChecklist(checklistId: number, name: string, isCompleted: boolean, isDeleted: boolean): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      checklistId: checklistId,
      name: name,
      isCompleted: isCompleted,
      isDeleted: isDeleted
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/checklist`, body, { 'headers': headers });
  }

  // API Get all user tasks
  getTaskFromAllProject(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/my-tasks?userId=` + this.author.currentUserValue?.id);
  }

  // API Search all user tasks
  searchAllUserTask(keyWord: string): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/search-tasks?userId=` + this.author.currentUserValue?.id + `&keyWord=` + keyWord);
  }

  // Api update subtask
  updateSubtask(currentSubtaskId: number, updateSubtaskId: number, taskId: number): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      currentSubtaskId: currentSubtaskId,
      updateSubtaskId: updateSubtaskId,
      taskId: taskId,
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/subtask`, body, { 'headers': headers });
  }

  //Get all label of project
  getAllLabel(projectId: number): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/label/label-project?projectId=` + projectId);
  }

  // Get all subtask for searching
  getAllSubtask(projectId: number): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/subtask?projectId=` + projectId);
  }

  // Get all task for searching task dependency
  getAllDenpendencyTask(projectId: number): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/dependency-task?projectId=` + projectId);
  }

  // Get all dependencies
  getAllDenpendencies(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/task/dependency`);
  }

  // Api create new dependency
  createNewDependency(taskDependId: number, dependencyId: number, taskId: number): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { taskDependId: taskDependId, dependencyId: dependencyId, taskId: taskId };
    const body = JSON.stringify(obj);
    return this.http.post<any>(`${environment.apiUrl}/api/dependency/create`, body, { 'headers': headers });
  }

  // Api update dependency
  updateDependency(taskDependencyId: number, taskDependId: number | null, dependencyId: number | null, isDeleted: boolean = false): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskDependencyId: taskDependencyId,
      taskDependId: taskDependId,
      dependencyId: dependencyId,
      isDeleted: isDeleted
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/dependency/informationn-dependency-task`, body, { 'headers': headers });
  }

  // Upload file in task
  uploadFile(taskId: string, fileToUpload: File) {
    const formData: FormData = new FormData();
    formData.append('taskId', taskId);
    formData.append('file', fileToUpload, fileToUpload.name);
    return this.http.post<any>(`${environment.apiUrl}/api/task/upload-file`, formData);
  }

  // Delete file in task
  deleteFileInTask(fileId: number, taskId: string): Observable<Task[]> {
    return this.http.delete<any>(`${environment.apiUrl}/api/task/file-attachment?idFileAttachment=` + fileId + `&taskId=` + taskId);
  }

  // API  Update Label for task
  updateLabel(TaskId: string, labelId: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: TaskId,
      labelId: labelId
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/label`, body, { 'headers': headers });
  }

  //API Update a status of task
  updateTaskStatus(taskId: string, statusId: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      taskId: taskId,
      statusId: statusId
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/status`, body, { 'headers': headers });
  }

  // API delete comment
  deleteCommentTask(commentId: string, content: string, attachFile: string, isDeleted: boolean): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      commentId: commentId,
      content: content,
      attachFile: attachFile,
      isDeleted: isDeleted,
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/comment`, body, { 'headers': headers });
  }

  //Update task comment
  updateTaskComment(commentId: string, content: string, attachFile: string, isDeleted: boolean): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      commentId: commentId,
      content: content,
      attachFile: attachFile,
      isDeleted: isDeleted
    };
    const body = JSON.stringify(obj);
    return this.http.put<any>(`${environment.apiUrl}/api/task/comment`, body, { 'headers': headers });
  }
}
