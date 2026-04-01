import {DynamicDialogRef} from 'primeng/dynamicdialog';

export class DeleteTaskModel {
  constructor(public taskId: number, public deleteModalRef: DynamicDialogRef) {}
}