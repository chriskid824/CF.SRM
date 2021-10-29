import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileModalFiletypeComponent } from './file-modal-filetype.component';

describe('FileModalFiletypeComponent', () => {
  let component: FileModalFiletypeComponent;
  let fixture: ComponentFixture<FileModalFiletypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FileModalFiletypeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FileModalFiletypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
