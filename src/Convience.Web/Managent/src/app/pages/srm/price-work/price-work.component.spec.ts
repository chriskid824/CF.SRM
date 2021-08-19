import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceWorkComponent } from './price-work.component';

describe('PriceWorkComponent', () => {
  let component: PriceWorkComponent;
  let fixture: ComponentFixture<PriceWorkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PriceWorkComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PriceWorkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
