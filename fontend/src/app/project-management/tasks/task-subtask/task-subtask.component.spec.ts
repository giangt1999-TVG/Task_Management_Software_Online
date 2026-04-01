import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskSubtaskComponent } from './task-subtask.component';

describe('TaskSubtaskComponent', () => {
  let component: TaskSubtaskComponent;
  let fixture: ComponentFixture<TaskSubtaskComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TaskSubtaskComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskSubtaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
