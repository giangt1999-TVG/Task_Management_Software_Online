import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from '@environments/environment';
import { Account } from '@app/_models/account';

@Injectable({
    providedIn: 'root'
})

export class AccountSerVice {

    constructor(private http: HttpClient) {
    }
    // API get All Account
    getListAccountInformation(): Observable<Account[]> {
        return this.http.get<Account[]>(`${environment.apiUrl}/api/account/account-information`);
    }

    async getUserInfoById(userID: string) {
        let params = new HttpParams()
            .set('userId', userID);
        return await this.http.get<any[]>(`${environment.apiUrl}/api/user/info`, { params }).toPromise();
    }

    getListRoles(): Observable<any[]> {
        return this.http.get<any[]>(`${environment.apiUrl}/api/account/role`);
    }

    searchAccountByUsername(keyWord: string): Observable<any> {
        let params = new HttpParams()
            .set('keyWord', keyWord);
        return this.http.get<any>(`${environment.apiUrl}/api/account/search-account`, { params });
    }

    deleteAccount(userID: string): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            id: userID,
            fullName: null,
            avatarUrl: null,
            description: null,
            isDeleted: true
        };
        const body = JSON.stringify(obj);
        return this.http.put<any>(`${environment.apiUrl}/api/user/user-information`, body, { 'headers': headers });
    }

    updateAccountInfo(userId: string, fullNAME: any | null, userName: any | null, rollNumber: any): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            id: userId,
            fullName: fullNAME,
            usename: userName,
            email: null,
            role: null,
            rollNumber: rollNumber
        };
        const body = JSON.stringify(obj);
        return this.http.put<any>(`${environment.apiUrl}/api/account/account-information`, body, { 'headers': headers });
    }

    addAccount(fullNAME: any | null, email: any | null, role: any | null, rollNumber: any | null): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            fullName: fullNAME,
            email: email,
            role: role,
            rollNumber: rollNumber
        };
        const body = JSON.stringify(obj);
        return this.http.post<any>(`${environment.apiUrl}/api/account/new`, body, { 'headers': headers });
    }

    changePassword(userId: string, oldPassword: string, newPassword: string): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        const obj = {
            userId: userId,
            oldPassword: oldPassword,
            newPassword: newPassword
        };
        const body = JSON.stringify(obj);
        return this.http.post<any>(`${environment.apiUrl}/api/account/change-password`, body, { 'headers': headers });
    }

    insertAccountFile(body: any): Observable<any> {
        const headers = { 'content-type': 'application/json' }
        return this.http.post<any>(`${environment.apiUrl}/api/account/import`, body, { 'headers': headers });
    }

    // private _listener = new Subject<any>();
    // listen(): Observable<any> {
    //     return this._listener.asObservable();
    // }
    // filter(filterBy: string){
    //     this._listener.next(filterBy);
    // }

}

