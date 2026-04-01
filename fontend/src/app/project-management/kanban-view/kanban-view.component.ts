import { NotificationService } from './../../_services/notification.service';
import { Role } from '@app/_models/role';
import { map } from 'rxjs/operators';
import { Section } from './../../_models/task';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { List } from './../../_models/list';
import { ListSectionTasks, ListTasks } from './../../_models/listTasks';
import { ListSerVice } from './../../_services/list.service';
import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService, PrimeNGConfig } from 'primeng/api';
import { TaskModelComponent } from '../tasks/task-model/task-model.component';
import { DialogService } from 'primeng/dynamicdialog';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { TaskService } from '@app/_services/task.service';
import { User } from '@app/_models/user';
import { AuthenticationService } from '@app/_services/authentication.service';
import { ProjectSerVice } from '@app/_services/project.service';

@Component({
    selector: 'app-kanban-view',
    templateUrl: './kanban-view.component.html',
    styleUrls: ['./kanban-view.component.scss'],
    providers: [DialogService]
})
export class KanbanViewComponent implements OnInit {

    lists: any = [];
    project: any = [];
    userRole!: string;
    listSectionTask: any = [];
    listSectionSubTask: any = [];
    kanbanForm!: FormGroup;
    currentUser!: User | null;
    isAddingSection!: boolean;
    isAddingTask!: boolean;
    isEditListName!: boolean;
    listIndexForTask!: number;
    listIndex!: number;
    listMode!: string;
    error = '';
    submitted = false;
    submittedTitle = false;
    subTaskMode: boolean = false;
    taskMode: boolean = false;
    allTaskMode: boolean = false;
    notifications: any = [];
    // listSection!: FormArray;

    constructor(private taskService: TaskService,
        private listService: ListSerVice,
        private primengConfig: PrimeNGConfig,
        private dialogService: DialogService,
        private activatedRoute: ActivatedRoute,
        private fb: FormBuilder,
        private authenticationService: AuthenticationService,
        private confirmationService: ConfirmationService,
        private projectService: ProjectSerVice,
        private messageService: MessageService) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    }

    //Get all section list
    async getMyLists() {
        let projectid = this.activatedRoute.snapshot.params.id;
        await this.listService.getAllList(projectid).then((res: any) => {
            this.lists = res.data;
        });
    }

    //Get information of project
    getProjectInformation() {
        let projectid = this.activatedRoute.snapshot.params.id;
        this.projectService.getProjectInformation(projectid).subscribe((res: any) => {
            this.project = res.data;
            this.project.listUser.map(o => {
                if (this.currentUser?.id === o.userId) {
                    this.userRole = o.role;
                }
            })
        });
    }

    setSubTaskMode() {
        this.subTaskMode = true;
        this.allTaskMode = true;
        this.taskMode = false;
    }

    setAllTaskMode() {
        this.allTaskMode = false;
        this.taskMode = false;
        this.subTaskMode = false;
    }

    setTaskMode() {
        this.taskMode = true;
        this.allTaskMode = true;
        this.subTaskMode = false;
    }

    //create new section
    createSection() {
        this.submitted = true;
        let projectid = this.activatedRoute.snapshot.params.id;
        const control = <FormArray>this.kanbanForm.get('listSection');
        if (this.f.sectionName.invalid) {
            return;
        } else {
            this.listService.createNewList(this.f.sectionName.value, projectid).subscribe((res: any) => {
                this.lists = res.data;
                this.getMyLists();
            },
                (error: any) => {
                    this.error = error;
                });
            control.push(this.fb.control(this.f.sectionName.value));
            this.setAddNewSection(false);
            this.f.sectionName.reset();
        }
    }

    //delete one section
    deleteList(listId: string, listName: string, i) {
        this.confirmationService.confirm({
            message: "Are you want to delete " + listName + "?",
            header: "Delete Confirmation",
            icon: "pi pi-info-circle",
            accept: () => {
                const control = <FormArray>this.kanbanForm.get('listSection');
                this.lists.splice(i, 1);
                this.listService.deleteList(listId).subscribe((res: any) => {
                    this.lists = res.data;
                    this.getMyLists();
                    control.removeAt(i);
                    this.messageService.add({ key: 'tr', severity: 'success', summary: 'Success', detail: 'Delete Section successfully' });
                },
                    (error: any) => {
                        this.error = error;
                    });
            },
            reject: () => {
                return;
            }
        })
    }

    //create new task in section
    createNewTask(listId: string) {
        this.submitted = true;
        if (this.f.taskName.invalid) {
            return;
        }
        else {
            this.taskService.createNewTask(listId, this.f.taskName.value).subscribe((res: any) => {
                this.lists = res.data;
                this.getMyLists();
                this.setAddNewTask(null, false);
                this.f.taskName.reset();
            },
                (error: any) => {
                    this.error = error;
                });
        }
    }

    //update new position of task in other section
    updateTaskPosition(taskID: string, listId: string) {
        this.taskService.updateTaskPosition(taskID, listId).subscribe((res: any) => {
            this.lists = res.data;
            this.getMyLists();
        },
            (error: any) => {
                this.error = error;
                this.getMyLists();
            });
    }

    //update section name
    updateListName(ListId: string, i) {
        this.submittedTitle = true;
        const control = <FormArray>this.kanbanForm.get('listSection');
        if (control.at(i).invalid) {
            return;
        } else {
            this.listService.updateListInfo(ListId, null, control.at(i).value).subscribe((res: any) => {
                this.lists = res.data;
                this.getMyLists();
            },
                (error: any) => {
                    this.error = error;
                });
        }
    }

    //update section index
    updateListPosition(ListId: string, Index: number) {
        this.listService.updateListInfo(ListId, Index, null).subscribe((res: any) => {
            this.lists = res.data;
            this.getMyLists();
        },
            (error: any) => {
                this.error = error;
            });
    }

    //Form validate
    createForm() {
        this.kanbanForm = this.fb.group({
            sectionName: ["", [
                Validators.required,
                Validators.maxLength(250)
            ]],
            taskName: ["", [
                Validators.required,
                Validators.maxLength(250)
            ]],
            listSection: new FormArray(this.lists.map(data => new FormControl(data.name, [
                Validators.required,
                Validators.maxLength(250)
            ])))
        });
    }

    getValidateRequierd(i) {
        return (<FormArray>this.kanbanForm.get('listSection')).controls[i].hasError('required');
    }

    getValidateMaxLength(i) {
        return (<FormArray>this.kanbanForm.get('listSection')).controls[i].hasError('maxlength');
    }

    async ngOnInit() {
        await this.getMyLists();
        this.getProjectInformation();
        this.primengConfig.ripple = true;
        this.listMode = "viewall";
        this.createForm();
    }

    //open add new section input
    setAddNewSection(mode: boolean) {
        this.submitted = false;
        this.isAddingSection = mode;
    }

    //close add new section input
    cancelAddNewSection() {
        this.setAddNewSection(false);
        this.f.sectionName.reset();
    }

    //open add new task input
    setAddNewTask(index, mode: boolean) {
        this.submitted = false;
        this.listIndexForTask = index;
        this.isAddingTask = mode;
    }

    //close add new task input
    cancelAddNewTask() {
        this.setAddNewTask(null, false);
        this.f.taskName.reset();
    }

    get f() { return this.kanbanForm.controls; }

    //drag task between sections
    dropTask(event: CdkDragDrop<ListTasks[]>, listID: string) {
        if (event.previousContainer === event.container) {
            moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        } else {
            transferArrayItem(event.previousContainer.data,
                event.container.data,
                event.previousIndex,
                event.currentIndex);
            const task = event.container.data[event.currentIndex];
            this.updateTaskPosition(task.taskId!, listID);
        }
    }

    //drag list to change position
    dropList(event: CdkDragDrop<List[]>) {
        moveItemInArray(this.lists, event.previousIndex, event.currentIndex);
        const listChange = event.container.data[event.currentIndex];
        const listIndex = event.currentIndex + 1;
        this.updateListPosition(listChange.listId!, listIndex);
        this.createForm();
    }

    //open task detail dialog
    openTaskModelDialog(taskId: number) {
        let dialogRef = this.dialogService.open(TaskModelComponent, {
            data: {
                taskId: taskId
            },
            showHeader: false,
            width: '1050px',
            contentStyle: {
                'padding': '15px 25px'
            },
            style: {
                'margin-left': '3%',
                'margin-right': '3%'
            },
            dismissableMask: false
        });
        dialogRef.onClose.subscribe(result => {
            this.getMyLists();
        });
    }

    //generate default avatar
    shortName(text: string) {
        var text_arr = text.split(" ");
        if (text_arr.length > 1) {
            return text_arr[0] + " " + text_arr[text_arr.length - 1];
        }
        return text_arr[0];
    }
}
