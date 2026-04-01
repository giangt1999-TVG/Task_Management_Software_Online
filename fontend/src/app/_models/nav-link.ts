export class SideBarLink {
    constructor(public name: string, public url?: string, public icon?: string) {}
}

let menu = new Map<string, SideBarLink[]>();
const adminMenu = [
    new SideBarLink('Accounts Management','account-list', 'pi-list')
];

const studentMenu = [
    new SideBarLink('Home', 'home-user', 'pi-home'),
    // new SideBarLink('List view', 'list-view'),
    // new SideBarLink('Kanban View','detail-project-kanban-view'),
    // new SideBarLink('Board', '#'),
    new SideBarLink('My Tasks', 'my-tasks', 'pi-list'),
    new SideBarLink('My Projects', 'my-projects', 'pi-bars'),
]

menu.set("Admin", adminMenu);
menu.set("Student", studentMenu);
menu.set("Teacher", studentMenu);

export default menu;

// export const SideBarLinks = [
//     new SideBarLink('Home', 'home-user'),
//     new SideBarLink('List view', '#'),
//     new SideBarLink('Kanban View','detail-project-kanban-view'),
//     new SideBarLink('Board', '#'),
//     new SideBarLink('Dashboard', '#'),
//     new SideBarLink('Project settings', '#')
// ];
  