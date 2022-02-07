import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EqplistComponent } from './eqplist.component';

describe('EqplistComponent', () => {
  let component: EqplistComponent;
  let fixture: ComponentFixture<EqplistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EqplistComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EqplistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
