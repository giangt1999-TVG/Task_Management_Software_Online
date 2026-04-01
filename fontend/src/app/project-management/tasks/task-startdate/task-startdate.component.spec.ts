import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskStartdateComponent } from './task-startdate.component';

describe('TaskStartdateComponent', () => {
  let component: TaskStartdateComponent;
  let fixture: ComponentFixture<TaskStartdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TaskStartdateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskStartdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
