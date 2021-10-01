import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EqpComponent } from './eqp.component';

describe('EqpComponent', () => {
  let component: EqpComponent;
  let fixture: ComponentFixture<EqpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EqpComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EqpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
