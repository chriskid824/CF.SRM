import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceManageComponent } from './price-manage.component';

describe('PriceManageComponent', () => {
  let component: PriceManageComponent;
  let fixture: ComponentFixture<PriceManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PriceManageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PriceManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
