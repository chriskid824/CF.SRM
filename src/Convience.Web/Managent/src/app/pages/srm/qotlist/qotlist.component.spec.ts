import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QotlistComponent } from './qotlist.component';

describe('QotlistComponent', () => {
  let component: QotlistComponent;
  let fixture: ComponentFixture<QotlistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QotlistComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QotlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
