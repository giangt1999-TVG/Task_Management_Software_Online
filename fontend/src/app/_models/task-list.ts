import { TaskStatus } from './task-status';
import { Assignee } from './assignee';
import { List } from "./list";

export interface TaskList {
    taskId?: string;
    name?: string;
    dueDate?: string;
    assignee?: Assignee;
    taskStatus?: TaskStatus;
    section?: List
}