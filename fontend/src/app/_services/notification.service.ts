import { map } from 'rxjs/operators';
import { Notifications, Content } from './../_models/notification';
import { AuthenticationService } from './authentication.service';
import { Project } from './../_models/project';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '@environments/environment';
const headers = { 'content-type': 'application/json' }

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private httpClient: HttpClient, private author: AuthenticationService) {
  }

  // Get all notification
  async getAllUserNoti() {
    return await this.httpClient.get<Notifications[]>(`${environment.apiUrl}/api/notification/` + this.author.currentUserValue?.id).toPromise();
  }

  // Read all notification
  readAllUserNoti(): Observable<any> {
    let body: any;
    return this.httpClient.post<any>(`${environment.apiUrl}/api/notification/read/` + this.author.currentUserValue?.id, body);
  }

  // Register user token
  registerToken(token: string | null, userId: string | undefined) : Observable<any> {
    
    const userToken = {
        userId: userId,
        token: token
    };

    const body = JSON.stringify(userToken);
    return this.httpClient.post<any>(`${environment.apiUrl}/api/notification/register-token`, body, { 'headers': headers });
  }
}