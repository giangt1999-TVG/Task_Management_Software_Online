import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, OnInit, Input } from '@angular/core';
import { Task } from '@app/_models/task';
import { TaskService } from '@app/_services/task.service';
import { ConfirmationService, MessageService } from "primeng/api";

@Component({
  selector: 'app-task-checklist',
  templateUrl: './task-checklist.component.html',
  styleUrls: ['./task-checklist.component.css']
})
export class TaskChecklistComponent implements OnInit {
  @Input() task!: Task;
  newCheckListForm!: FormGroup;
  submitted: boolean = false;
  isPending: boolean = false;
  displayNewPopup: boolean = false;
  displayUpdatePopup: boolean = false;
  updateCheckListForm!: FormGroup;
  currentChecklistIdUpdated!: number;
  numberOfTaskDone!: number;
  numberofTask!: number;

  constructor(private formBuilder: FormBuilder,
    private _taskService: TaskService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService) { }

  ngOnInit(): void {
    this.numberOfTaskDone = this.task.checklists.filter(c => c.isCompleted == true).length;
    this.numberofTask = this.task.checklists.length;

    this.newCheckListForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
    });

    this.updateCheckListForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
    });
  }

  showNewDialog() {
    this.submitted = false;
    this.newCheckListForm.reset();
    this.displayNewPopup = true;
  }

  closeNewDialog() {
    this.displayNewPopup = false;
  }

  showUpdateDialog(checklistId: number) {
    this.currentChecklistIdUpdated = checklistId;
    this.submitted = false;
    var checklist = this.task.checklists.find(c => c.checklistId == checklistId);
    this.u.name.setValue(checklist?.name);
    this.displayUpdatePopup = true;
  }

  closeUpdateDialog() {
    this.displayUpdatePopup = false;
  }

  createNewCheckList() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.newCheckListForm.invalid) {
        return;
    }

    this._taskService.createNewChecklist(this.f.name.value, parseInt(this.task.taskId)).subscribe((res: any) => {
      this.numberofTask++;
      this.task.checklists.push(res.data);
    });

    this.closeNewDialog();
  }

  toggleCheckbox(value: any, checklistId: number) {
    value.originalEvent.stopPropagation();

    if (value.checked) {
      this.numberOfTaskDone++;
    } else {
      this.numberOfTaskDone--;
    }

    this._taskService.updateChecklist(checklistId, "", value.checked, false).subscribe((res: any) => {
      var checklist = this.task.checklists.find(c => c.checklistId == checklistId);
      if (checklist) checklist.isCompleted = res.data.isCompleted;
    });
  }

  confirmDelete(event: Event, checklistId: number) {
    event.stopPropagation();
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      key: "deleteChecklistPopup",
      message: "Are you sure that you want to delete it?",
      icon: "pi pi-exclamation-triangle",
      accept: () => {
        var checklist = this.task.checklists.find(c => c.checklistId == checklistId);
        if (checklist) {
          this._taskService.updateChecklist(checklistId, "", checklist.isCompleted, true).subscribe((res: any) => {
            this.task.checklists.splice(this.task.checklists.findIndex(c => c.checklistId === checklistId) , 1);
            this.numberofTask--;
          });

          this.messageService.add({
            severity: "info",
            summary: "Delete successfully!"
          });
        }
      },
      reject: () => {
      }
    });
  }

  // Update checklist information
  updateCheckList(checklistId: number) {
    this.submitted = true;

    // stop here if form is invalid
    if (this.updateCheckListForm.invalid) {
        return;
    }

    var checklist = this.task.checklists.find(c => c.checklistId == checklistId);
    if (checklist) {
      this._taskService.updateChecklist(checklistId, this.u.name.value, checklist.isCompleted, false).subscribe((res: any) => {
        if (checklist) checklist.name = res.data.name;
      });
    }

    this.closeUpdateDialog();
  }

  // Convenience getter for easy access to form fields
  get f() { return this.newCheckListForm.controls; }

  // Convenience getter for easy access to form fields
  get u() { return this.updateCheckListForm.controls; }

}
