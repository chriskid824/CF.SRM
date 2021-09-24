import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoExamineComponent } from './po-examine.component';

describe('PoExamineComponent', () => {
  let component: PoExamineComponent;
  let fixture: ComponentFixture<PoExamineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoExamineComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoExamineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
