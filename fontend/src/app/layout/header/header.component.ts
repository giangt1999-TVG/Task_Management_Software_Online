import { UserService } from './../../_services/user.service';
import { NotificationService } from './../../_services/notification.service';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { MatDialog } from '@angular/material/dialog';
import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

import { AuthenticationService } from '@app/_services/authentication.service';
import { User } from '@app/_models/user';
import { DialogService } from 'primeng/dynamicdialog';
import { ChangePasswordComponent } from './change-password/change-password.component';
// import { parse } from 'path';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  providers: [DialogService]
})
export class HeaderComponent implements OnInit {
  displayUserMenu: boolean;
  shortName!: string | undefined;
  currentUser!: User | null;
  notifications: any = [];
  unread: number = 0;
  isViewed = 'is-viewed';
  notViewed = 'not-viewed'
  viewed: boolean = false;
  userInfo!: User;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private matDialog: MatDialog,
    private notificationService: NotificationService,
    private userService: UserService,
    private dialogService: DialogService
  ) {
    this.displayUserMenu = false;
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
  }

  async getAllUserNotification() {
    await this.notificationService.getAllUserNoti().then((res: any) => {
      this.notifications = res.data;
      for (let noti of this.notifications) {
        noti.content = JSON.parse(noti.content);
        noti.createdDate = new Date(noti.createdDate);
        noti.isViewed = noti.isViewed;
        if (noti.isViewed == false) {
          this.unread = this.unread + 1;
        }
      }
    });
  }

  async ngOnInit(): Promise<void> {
    this.shortName = this.getShortName(this.currentUser?.fullName);
    await this.getAllUserNotification();
  }

  markAsRead() {
    this.notViewed = 'is-viewed';
    this.unread = 0;
    this.viewed = true;
    this.notificationService.readAllUserNoti().subscribe((res: any) => {
      this.notifications = res.data;
      this.getAllUserNotification();
    });
  }

  showUserMenu() {
    this.displayUserMenu = !this.displayUserMenu;
  }

  private getShortName(text) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  // Open User Profile Dialog
  openUserProfileDialog() {
    let dialogRef = this.matDialog.open(UserProfileComponent, { autoFocus: false });
    dialogRef.afterClosed().subscribe(result => {
      this.shortName = this.getShortName(this.currentUser?.fullName);
    });
    dialogRef.afterClosed().subscribe(result => {
      const returnedResult = result.data;
      this.getUserInfo();
    });
  }

  //API get User Info
  getUserInfo() {
    this.userService.getUserInfoById().subscribe((res: any) => {
      this.userInfo = res.data;
      this.currentUser!.fullName = this.userInfo.fullName;
      this.currentUser!.avatarUrl = this.userInfo.avatarUrl;
    });
  }

  openChangePasswordDialog() {
    let dialogRef = this.dialogService.open(ChangePasswordComponent, {
      data: {
        userId: this.currentUser?.id
      },
      showHeader: true,
      header: 'Change Password',
      width: '350px',
      contentStyle: {
        'padding': '0px 25px 15px 25px'
      },
      style: {
        'margin-left': '3%',
        'margin-right': '3%'
      },
      dismissableMask: false
    });
  }
}
