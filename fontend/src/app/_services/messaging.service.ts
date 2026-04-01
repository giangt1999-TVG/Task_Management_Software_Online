import { Injectable } from '@angular/core';
import { AngularFireMessaging } from '@angular/fire/messaging';
import { BehaviorSubject } from 'rxjs'
import { MessageService } from 'primeng/api';
import { AuthenticationService } from './authentication.service';
import { User } from '@app/_models/user';
import { HttpClient } from '@angular/common/http';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class MessagingService {
  currentMessage = new BehaviorSubject(null);
  currentUser!: User | null;

  constructor(private angularFireMessaging: AngularFireMessaging,
    private messageService: MessageService,
    private authenticationService: AuthenticationService,
    private httpClient: HttpClient,
    private notificationService: NotificationService) {

      this.authenticationService.currentUser.subscribe(x => this.currentUser = x);

      this.angularFireMessaging.messages.subscribe(
        (_messaging: AngularFireMessaging | any) => {
          _messaging.onMessage = _messaging.onMessage.bind(_messaging);
          _messaging.onTokenRefresh = _messaging.onTokenRefresh.bind(_messaging);
        }
      );
  }

  requestPermission() {
    if (this.currentUser != null) {
      this.angularFireMessaging.requestToken.subscribe(
        (token) => {
          this.notificationService.registerToken(token, this.currentUser?.id).subscribe(
            () => {},
            (err) => {}
          );
        },
        (err) => {
          console.error('Unable to get permission to notify.', err);
        }
      );
    }
  }

  receiveMessage() {
    this.angularFireMessaging.messages.subscribe(
      (payload: any) => {
        this.messageService.add({ key: 'notifications', severity: 'info', summary: payload.notification.title, detail: payload.notification.body });
        this.currentMessage.next(payload);
      })
  }
}
