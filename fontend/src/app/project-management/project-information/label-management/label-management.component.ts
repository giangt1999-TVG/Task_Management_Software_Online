import { isNull } from '@angular/compiler/src/output/output_ast';
import { isEmpty } from 'rxjs/operators';
import { MessageService } from 'primeng/api';
import { ProjectSerVice } from '@app/_services/project.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, Inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-label-management',
  templateUrl: './label-management.component.html',
  styleUrls: ['./label-management.component.css']
})
export class LabelManagementComponent implements OnInit {

  addNewLabelForm!: FormGroup;
  color!: string;
  listLabel: any[] = [];
  labelName!: string;
  projectId: any;
  submitted = false;
  colorLabel!: string;

  constructor(@Inject(MAT_DIALOG_DATA) data, private formBuider: FormBuilder, private projectService: ProjectSerVice, private messageService: MessageService, private matDialog: MatDialog) {
    this.projectId = data.projectId;
  }

  ngOnInit(): void {
    this.addNewLabelForm = this.formBuider.group({
      labelName: ["", [Validators.required, Validators.minLength(1), Validators.pattern(/([a-zA-Z0-9])$/), Validators.maxLength(50)]],
      color: ["",[Validators.required]],
    })
  }

  // Convenience getter for easy access to form fields
  get f() { return this.addNewLabelForm.controls; }

  //Get color 
  getColorLabel() {
    if (this.f.color.value !== '') {
      this.colorLabel = this.f.color.value.slice(1, 7);
    }
    return this.colorLabel;
  }

  //Add new label
  addNewLabel() {
    this.submitted = true;
    if (this.addNewLabelForm.invalid) {
      return;
    }
    else {
      this.getColorLabel();
      if (this.f.labelName.value !== '' && this.getColorLabel()) {
        this.projectService.createNewLabelProject(this.f.labelName.value.trim(), this.getColorLabel(), this.projectId).subscribe((res: any) => {
          this.listLabel = res.data;
          this.matDialog.closeAll();
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Add New Label successfully' });
        });
      }
      else {
        return;
      }
    }
  }

  closeDialog() {
    this.matDialog.closeAll();
  }

}
