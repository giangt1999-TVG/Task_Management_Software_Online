import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-tab-menu',
  templateUrl: './tab-menu.component.html',
  styleUrls: ['./tab-menu.component.scss']
})
export class TabMenuComponent implements OnInit {

  items!: MenuItem[];
  activeItem!: MenuItem;
  constructor(private activatedRoute: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    let projectid = this.activatedRoute.snapshot.params.id;
    this.items = [
      {
        label: 'Kanban View',
        routerLink: ['kanban-view/', projectid],
        routerLinkActiveOptions: { exact: true }
      },
      {
        label: 'List View',
        routerLink: ['list-view/', projectid],
        routerLinkActiveOptions: { exact: true }
      },
      {
        label: 'Project Information',
        routerLink: ['project-information/', projectid],
        routerLinkActiveOptions: { exact: true }
      }
    ];
    if (this.router.url === "/project/" + projectid + "/kanban-view/" + projectid) {
      this.activeItem = this.items[0];
    }
    else if (this.router.url === "/project/" + projectid + "/list-view/" + projectid) {
      this.activeItem = this.items[1];
    }
    else if (this.router.url === "/project/" + projectid + "/project-information/" + projectid) {
      this.activeItem = this.items[2];
    }
  }

}
