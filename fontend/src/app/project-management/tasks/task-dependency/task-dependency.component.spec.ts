import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskDependencyComponent } from './task-dependency.component';

describe('TaskDependencyComponent', () => {
  let component: TaskDependencyComponent;
  let fixture: ComponentFixture<TaskDependencyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TaskDependencyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskDependencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
