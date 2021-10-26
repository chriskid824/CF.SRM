import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialBatchUploadComponent } from './material-batch-upload.component';

describe('MaterialBatchUploadComponent', () => {
  let component: MaterialBatchUploadComponent;
  let fixture: ComponentFixture<MaterialBatchUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MaterialBatchUploadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialBatchUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
