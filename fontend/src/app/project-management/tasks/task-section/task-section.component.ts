import { MessageService } from 'primeng/api';
import { ListSerVice } from '@app/_services/list.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '@app/_models/task';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-task-section',
  templateUrl: './task-section.component.html',
  styleUrls: ['./task-section.component.css']
})
export class TaskSectionComponent implements OnInit {
  @Input() task!: Task;
  sections!: any[];
  currentSection: any;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;
  projectId: any;

  constructor(private listService: ListSerVice, private activatedRoute: ActivatedRoute, private messageService: MessageService) {
  }

  getProjectID() {
    let projectid = this.activatedRoute.snapshot.params.id;
    return projectid;
  }

  ngOnInit(): void {
    this.projectId = this.task.projectId;
    this.getListSectionProject(this.task.projectId);
  }

  // API change section of task
  onChangeSection(selectedSection) {
    if (selectedSection.id !== this.task.section.sectionId) {
      this.listService.updateTaskPositionInSection(this.task.taskId, selectedSection.id).subscribe((res: any) => {
        this.task.section = res.data;
        this.showSuccessMessage();
        this.uploadStatus = true;
        this.emitter.emit(this.uploadStatus);
      });
    }
    else
      return;
  }

  // API get list section
  getListSectionProject(projectId: string) {
    this.listService.getListSection(projectId).subscribe((res: any) => {
      this.sections = res.data;
    });
  }

  // Toast message success
  showSuccessMessage() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Section of task is updated!' });
  }
}
