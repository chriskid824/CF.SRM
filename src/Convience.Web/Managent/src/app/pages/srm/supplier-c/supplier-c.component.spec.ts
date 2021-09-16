import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierCComponent } from './supplier-c.component';

describe('SupplierCComponent', () => {
  let component: SupplierCComponent;
  let fixture: ComponentFixture<SupplierCComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SupplierCComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
