import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskAssigneeComponent } from './task-assignee.component';

describe('TaskAssigneeComponent', () => {
  let component: TaskAssigneeComponent;
  let fixture: ComponentFixture<TaskAssigneeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TaskAssigneeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskAssigneeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
