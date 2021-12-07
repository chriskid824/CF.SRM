import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessManageComponent } from './process-manage.component';

describe('ProcessManageComponent', () => {
  let component: ProcessManageComponent;
  let fixture: ComponentFixture<ProcessManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProcessManageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcessManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
