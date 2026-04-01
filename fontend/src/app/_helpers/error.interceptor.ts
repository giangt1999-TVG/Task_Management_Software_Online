import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {MessageService} from 'primeng/api';

import { AuthenticationService } from '@app/_services/authentication.service';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService,
        private messageService: MessageService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if ([401, 403].indexOf(err.status) !== -1) {
                this.authenticationService.logout();
                location.reload(true);
            } else if ([404, 0].indexOf(err.status) !== -1) {
                this.messageService.add({key: 'error-message', severity:'error', summary:'Error', detail:'An error occurred while connecting to the server!'});
                return throwError('');
            }

            return throwError(err);
        }))
    }
}