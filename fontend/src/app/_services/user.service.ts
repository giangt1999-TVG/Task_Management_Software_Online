import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from "@angular/core";
import { Student } from '@app/_models/student';
import { environment } from '@environments/environment';
import { Teacher } from '@app/_models/teacher';
import { User } from '@app/_models/user';
import { AuthenticationService } from './authentication.service';


@Injectable({
    providedIn: 'root'
})

export class UserService{

    constructor(private httpClient: HttpClient, private author: AuthenticationService){

    }

    // API get All Student
    getAllStudent(): Observable<Student[]> {
        return this.httpClient.get<Student[]>(`${environment.apiUrl}/api/user/students`);
    }

    //API get All Teacher
    getAllTeacher(): Observable<Teacher[]> {
        return this.httpClient.get<Teacher[]>(`${environment.apiUrl}/api/user/teachers`);
    }

    // API get User Info By ID
    getUserInfoById(): Observable<any[]> {
        return this.httpClient.get<any[]>(`${environment.apiUrl}/api/user/info?userId=` + this.author.currentUserValue?.id);
    }

    // API update user information
    updateInfomationUserAPI(id: string, fullName: string, avatarUrl: string, description: string, isDeleted: boolean): Observable<any>{
        const headers = { 'content-type': 'application/json' }
        const obj = {
            id: id,
            fullName: fullName,
            avatarUrl: avatarUrl,
            description: description,
            isDeleted: isDeleted,
        };
        const body = JSON.stringify(obj);
        return this.httpClient.put<any>(`${environment.apiUrl}/api/user/user-information`, body, { 'headers': headers });
    }

    // API Get all users in project
    getUserInProject(projectId: string): Observable<any[]> {
        return this.httpClient.get<any[]>(`${environment.apiUrl}/api/user?projectId=` + projectId);
    }

    // API information which is assigneed
    updateUserAssigneeTask(taskId: string, userId: string): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            taskId: taskId,
            userId: userId
        };
        const body = JSON.stringify(obj);
        return this.httpClient.put<any>(`${environment.apiUrl}/api/task/informationn-assignee`, body, { 'headers': headers });
    }
}