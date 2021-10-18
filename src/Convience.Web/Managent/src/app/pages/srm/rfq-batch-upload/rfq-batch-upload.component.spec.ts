import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RfqBatchUploadComponent } from './rfq-batch-upload.component';

describe('RfqBatchUploadComponent', () => {
  let component: RfqBatchUploadComponent;
  let fixture: ComponentFixture<RfqBatchUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RfqBatchUploadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RfqBatchUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
