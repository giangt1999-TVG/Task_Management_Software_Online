import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { List } from '@app/_models/list';

@Injectable({
    providedIn: 'root'
})

export class ListSerVice {

    constructor(private http: HttpClient) {
    }
    // API get All List and Task
    async getAllList(projectID: string) {
        return await this.http.get<List[]>(`${environment.apiUrl}/api/list/list-task?projectId=` + projectID).toPromise();
    }

    // API create new List
    createNewList(taskName: string, ProjectId: string): Observable<any> {

        let body: any;
        let params = new HttpParams()
            .set('Name', taskName)
            .set('ProjectId', ProjectId);

        return this.http.post<any>(`${environment.apiUrl}/api/list/create`, body, { params });
    }

    // API delete List
    deleteList(listId: string): Observable<any> {
        let params = new HttpParams()
            .set('id', listId)
            .set('projectId', '13');
        return this.http.delete<any>(`${environment.apiUrl}/api/list/delete-section`, { params });
    }

    // API update List information
    updateListInfo(ListId: string, Index: number | null, listName: string | null): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            id: ListId,
            index: Index,
            name: listName
        };
        const body = JSON.stringify(obj);
        return this.http.put<any>(`${environment.apiUrl}/api/list`, body, { 'headers': headers });
    }

    //API Get list section in project
    getListSection(projectId: string): Observable<any> {
        return this.http.get<any>(`${environment.apiUrl}/api/list/sections?projectId=` + projectId);
    }

    // API Update task position in section
    updateTaskPositionInSection(taskId: string, listId: string): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            taskId: taskId,
            listId: listId
        };
        const body = JSON.stringify(obj);
        return this.http.put<any>(`${environment.apiUrl}/api/task/task-position`, body, { 'headers': headers });
    }
}

