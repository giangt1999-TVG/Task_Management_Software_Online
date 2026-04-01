import { Assignee } from './assignee';
export interface ListTasks {
    taskId?: string;
    name?: string;
    category?: string;
    dueDate?: string;
    assignee?: Assignee;
}

export interface ListSectionTasks {
    taskId?: string;
    name?: string;
    category?: string;
    dueDate?: string;
    assignee?: Assignee;
}

export interface ListSubTasks {
    taskId?: string;
    name?: string;
    category?: string;
    dueDate?: string;
    assignee?: Assignee;
}
