import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoSapComponent } from './po-sap.component';

describe('PoSapComponent', () => {
  let component: PoSapComponent;
  let fixture: ComponentFixture<PoSapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoSapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoSapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
