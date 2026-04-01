import { AuthenticationService } from './authentication.service';
import { Project } from './../_models/project';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '@environments/environment';


@Injectable({
  providedIn: 'root'
})

export class ProjectSerVice {

  private currentProjectSubject!: BehaviorSubject<Project | null>;
  public currentProject!: Observable<Project | null>;

  constructor(private httpClient: HttpClient, private author: AuthenticationService) {
    this.currentProjectSubject = new BehaviorSubject<Project | null>(JSON.parse(localStorage.getItem('currentProject') || "null"));
    this.currentProject = this.currentProjectSubject.asObservable();
  }
  // API get All Project
  getAllProject(): Observable<Project[]> {
    return this.httpClient.get<Project[]>(`${environment.apiUrl}/api/project/my-projects?userId=` + this.author.currentUserValue?.id);
  }

  //API create new project
  createNewProject(projectCode: string, projectName: string, description: string, startDate: Date, endDate: Date, teacherId: string, teamleadId: string, memberIds: any[]): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { projectCode: projectCode, projectName: projectName, description: description, startDate: startDate, endDate: endDate, teacherId: teacherId, teamleadId: teamleadId, memberIds: memberIds };
    const body = JSON.stringify(obj);
    return this.httpClient.post<any>(`${environment.apiUrl}/api/project/create`, body, { 'headers': headers });
  }

  public get currentProjectValue(): Project | null {
    return this.currentProjectSubject.value;
  }

  //API Get project information
  getProjectInformation(projectId: string): Observable<Project[]> {
    return this.httpClient.get<Project[]>(`${environment.apiUrl}/api/project/information/` + projectId);
  }

  // API Delete project member
  deleteProjectMember(projectId: string, userId: string): Observable<any> {
    const obj = {
      projectId: projectId,
      userId: userId,
    };
    const body = JSON.stringify(obj);
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body
    };
    return this.httpClient.delete<any>(`${environment.apiUrl}/api/project/delete-member`, options);
  }

  // API Delete project label
  deleteProjectLabel(projectId: string, labelId: string): Observable<any> {
    const obj = {
      projectId: projectId,
      labelId: labelId,
    };
    const body = JSON.stringify(obj);
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body
    };
    return this.httpClient.delete<any>(`${environment.apiUrl}/api/project/delete-label`, options);
  }

  // API Add project member
  createNewMemberProject(projectId: string, userId: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { projectId: projectId, userId: userId };
    const body = JSON.stringify(obj);
    return this.httpClient.post<any>(`${environment.apiUrl}/api/project/add-member`, body, { 'headers': headers });
  }

  // API Add new label
  createNewLabelProject(name: string, color: string, projectId: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = { name: name, color: color, projectId: projectId };
    const body = JSON.stringify(obj);
    return this.httpClient.post<any>(`${environment.apiUrl}/api/label/create`, body, { 'headers': headers });
  }

  // API Seach Project By Code
  searchProjectByCode(keyWord: string): Observable<any> {
    return this.httpClient.get<any>(`${environment.apiUrl}/api/project/search-projects?userId=` + this.author.currentUserValue?.id + `&keyWord=` + keyWord);
  }

  // API update project information
  updateProjectInformation(projectId: string, projectCode: string, projectName: string, description: string, startDate: Date, endDate: Date): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      projectId: projectId,
      projectCode: projectCode,
      projectName: projectName,
      description: description,
      startDate: startDate,
      endDate: endDate
    };
    const body = JSON.stringify(obj);
    return this.httpClient.put<any>(`${environment.apiUrl}/api/project/update-info`, body, { 'headers': headers });
  }

  // Update teacher and teamlead in project
  updateTeacherAndTeamleadInProject(projectId: string, oldUserId: string, newUserId: string, role: string): Observable<any> {
    const headers = { 'content-type': 'application/json' }
    const obj = {
      projectId: projectId,
      oldUserId: oldUserId,
      newUserId: newUserId,
      role: role
    };
    const body = JSON.stringify(obj);
    return this.httpClient.put<any>(`${environment.apiUrl}/api/project/update-member`, body, { 'headers': headers });
  }

}