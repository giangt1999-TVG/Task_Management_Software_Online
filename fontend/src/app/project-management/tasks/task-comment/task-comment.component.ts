import { MessageService, ConfirmationService } from 'primeng/api';
import { TaskService } from '@app/_services/task.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Comment, Task } from '@app/_models/task';
import { User } from '@app/_models/user';
import { AuthenticationService } from '@app/_services/authentication.service';

@Component({
  selector: 'app-task-comment',
  templateUrl: './task-comment.component.html',
  styleUrls: ['./task-comment.component.css'],
  providers: [ConfirmationService]

})
export class TaskCommentComponent implements OnInit {
  @Input() task!: Task;
  @Input() comment!: Comment;
  @Input() createMode!: boolean;
  isEditing!: boolean;
  currentUser!: User | null;
  shortName!: string | undefined;
  commentText!: string;
  createNewCommentForm!: FormGroup;
  updateCommentForm!: FormGroup;
  attachFile: string = "string";
  userId!: any;
  edittingComment = false;
  check = false;
  @Output() emitter: EventEmitter<any[]> = new EventEmitter();
  uploadStatus: any;

  constructor(private authenticationService: AuthenticationService, private formBuider: FormBuilder, private taskService: TaskService, private messageService: MessageService, private confirmationService: ConfirmationService) {
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    if (this.createMode) {
      this.shortName = this.getShortName(this.currentUser?.fullName);
    } else {
      this.shortName = this.getShortName(this.comment.author.fullName);
    }
    this.createNewCommentForm = this.formBuider.group({
      commentText: ['', [Validators.minLength(3), Validators.maxLength(1000)]],
    });

    this.updateCommentForm = this.formBuider.group({
      commentTextUpdate: ['', [Validators.minLength(3), Validators.maxLength(1000)]],
    });
  }

  setCommentEdit(mode: boolean) {
    this.isEditing = mode;
  }

  setCommentUpdateEdit(mode: boolean) {
    this.isEditing = mode;
  }

  // Convenience getter for easy access to form fields
  get f() { return this.createNewCommentForm.controls; }

  // Convenience getter for easy access to form fields
  get g() { return this.updateCommentForm.controls; }

  getIdCurrentUser() {
    this.userId = this.currentUser?.id;
    return this.userId;
  }

  // Add new comment
  addComment() {
    this.taskService.createNewComment(this.getIdCurrentUser(), this.task.taskId, this.f.commentText.value, this.attachFile).subscribe((res: any) => {
      this.task.comments.push(res.data);
    });
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Comment successfully' });
    this.setCommentEdit(false);
    this.f.commentText.setValue("");
    this.uploadStatus = true;
    this.emitter.emit(this.uploadStatus);
  }

  // Cancel add comment
  cancelAddComment() {
    this.setCommentEdit(false);
    this.f.commentText.setValue("");
  }

  // API edit comment
  editCommentText(commentId) {
    var comment = this.task.comments.find(t => t.commentId == commentId)
    this.taskService.updateTaskComment(commentId, this.g.commentTextUpdate.value, "null", false).subscribe((res: any) => {
      if (comment) {
        comment.content = res.data.content;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Update comment successfully' });
      }
    });
    this.setCommentEdit(false);
    this.createMode = false;
    this.edittingComment = false;
  }

  // Edit comment
  editComment(commentId) {
    this.createMode = true;
    this.edittingComment = true;
    this.check = true;
    this.setCommentEdit(false);
    this.g.commentTextUpdate.setValue(this.comment.content);
  }

  cancelEditComment() {
    this.g.commentTextUpdate.setValue(this.comment.content);
    this.edittingComment = false;
    this.setCommentEdit(false);
    this.createMode = false;
  }

  //Delete comment
  deleteComment(commentId) {
    this.confirmationService.confirm({
      message: "Do you want to delete this comment?",
      header: "Delete Confirmation",
      icon: "pi pi-info-circle",
      accept: () => {
        this.taskService.deleteCommentTask(commentId, "null", "null", true).subscribe((res: any) => {
          this.comment = res.data.comment;
          this.task.comments.splice(this.task.comments.findIndex(c => c.commentId === commentId), 1);
          this.showSuccess();
        });
      },
      reject: () => {
        return;
      }
    })
  }

  private getShortName(text) {
    var text_arr = text.split(" ");
    if (text_arr.length > 1) {
      return text_arr[0] + " " + text_arr[text_arr.length - 1];
    }
    return text_arr[0];
  }

  // Toast message success
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Comment is deleted!' });
  }
}
