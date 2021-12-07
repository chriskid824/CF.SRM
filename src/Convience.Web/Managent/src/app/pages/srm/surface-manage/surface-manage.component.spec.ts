import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SurfaceManageComponent } from './surface-manage.component';

describe('SurfaceManageComponent', () => {
  let component: SurfaceManageComponent;
  let fixture: ComponentFixture<SurfaceManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SurfaceManageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SurfaceManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
