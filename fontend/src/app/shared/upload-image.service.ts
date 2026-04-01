import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from '@environments/environment';

@Injectable({
    providedIn: 'root'
})

export class UploadImageService{
    constructor(private httpClient: HttpClient){
    }

    // API upload image file
    uploadImageFile(fileToUpload: File){
        const formData: FormData = new FormData();
        formData.append('file', fileToUpload, fileToUpload.name);
        return this.httpClient.post<any>(`${environment.apiUrl}/api/account/avatar`, formData);
    }
}