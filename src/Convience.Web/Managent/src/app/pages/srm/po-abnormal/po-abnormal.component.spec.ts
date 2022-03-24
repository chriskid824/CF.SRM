import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoAbnormalComponent } from './po-abnormal.component';

describe('PoAbnormalComponent', () => {
  let component: PoAbnormalComponent;
  let fixture: ComponentFixture<PoAbnormalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoAbnormalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoAbnormalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
