import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QotComponent } from './qot.component';

describe('QotComponent', () => {
  let component: QotComponent;
  let fixture: ComponentFixture<QotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QotComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
