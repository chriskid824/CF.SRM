import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierBatchUploadComponent } from './supplier-batch-upload.component';

describe('SupplierBatchUploadComponent', () => {
  let component: SupplierBatchUploadComponent;
  let fixture: ComponentFixture<SupplierBatchUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SupplierBatchUploadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierBatchUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
