import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DelyveryLComponent } from './delyvery-l.component';

describe('DelyveryLComponent', () => {
  let component: DelyveryLComponent;
  let fixture: ComponentFixture<DelyveryLComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DelyveryLComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DelyveryLComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
