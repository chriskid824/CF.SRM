import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RfqManageComponent } from './rfq-manage.component';

describe('RfqManageComponent', () => {
  let component: RfqManageComponent;
  let fixture: ComponentFixture<RfqManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RfqManageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RfqManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
