import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeliveryReceiveComponent } from './delivery-receive.component';

describe('DeliveryReceiveComponent', () => {
  let component: DeliveryReceiveComponent;
  let fixture: ComponentFixture<DeliveryReceiveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeliveryReceiveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeliveryReceiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
