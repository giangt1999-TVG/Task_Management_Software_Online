import { Component, OnInit, Input } from '@angular/core';
import { SideBarLink } from '@app/_models/nav-link';
import menu from '@app/_models/nav-link';
import { AuthenticationService } from '@app/_services/authentication.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  @Input() expanded!: boolean;

  sideBarLinks!: SideBarLink[] | undefined;

  get sidebarWidth(): number {
    return this.expanded ? 240 : 15;
  }

  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    var userRole = this.authenticationService.currentUserValue?.mainRole
    if (userRole === "Admin") {
      this.sideBarLinks = menu.get("Admin");
    } else if (userRole === "Teacher") {
      this.sideBarLinks = menu.get("Teacher");
    } else if (userRole === "Student") {
      this.sideBarLinks = menu.get("Student");
    }
  }

}
