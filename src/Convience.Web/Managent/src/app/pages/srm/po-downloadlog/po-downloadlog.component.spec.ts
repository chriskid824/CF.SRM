import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoDownloadlogComponent } from './po-downloadlog.component';

describe('PoDownloadlogComponent', () => {
  let component: PoDownloadlogComponent;
  let fixture: ComponentFixture<PoDownloadlogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoDownloadlogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoDownloadlogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
