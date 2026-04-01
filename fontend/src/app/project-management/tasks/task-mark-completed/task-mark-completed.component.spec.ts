import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskMarkCompletedComponent } from './task-mark-completed.component';

describe('TaskMarkCompletedComponent', () => {
  let component: TaskMarkCompletedComponent;
  let fixture: ComponentFixture<TaskMarkCompletedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TaskMarkCompletedComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskMarkCompletedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
