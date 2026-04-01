import { Type } from "@angular/core";

export enum TaskType {
    TASK = 'Task',
    SUBTASK = 'Subtask'
}

export interface Priority {
    priorityId: number;
    name: string;
}

export interface Status {
    statusId: number;
    name: string;
}

export interface Section {
    sectionId: number;
    name: string;
}

export interface Assignee {
    userId: string;
    fullName: string;
    avatarUrl: string;
}

export interface Subtask {
    taskId: number;
    name: string;
    statusName: string;
}

export interface Author {
    userId: string;
    userName: string;
    fullName?: string;
    avatarUrl?: string;
}

export interface Comment {
    commentId: number;
    author: Author;
    content: string;
    attachFile: string;
    createdDate: string;
    modifiedDate: string;
}

export interface Checklist {
    checklistId: number;
    name: string;
    isCompleted: boolean;
    createdDate: string;
    modifiedDate: string;
}

export interface Dependency {
    dependencyId: number;
    taskDenpendencyId: number;
    taskName: string;
    dependencyName: string;
    createdDate: string;
}

export interface Label {
    labelId: number;
    name: string;
    color: string;
}

export interface FileAttachment {
    fileAttachmentId: number;
    name: string;
    mediaLink: string;
    createdDate: string;
}

export interface Task {
    projectId: string;
    taskId:string;
    name:string;
    description:string;
    projectName: string;
    startDate:string;
    dueDate:string;
    isCompleted:boolean;
    isActive:boolean;
    parentId?:number;
    priority: Priority;
    status: Status;
    section: Section;
    assignee: Assignee;
    subtasks: Subtask[];
    comments: Comment[];
    checklists: Checklist[];
    dependencies: Dependency[];
    labels: Label[];
    fileAttachments: FileAttachment[];
}