import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoDetail1200Component } from './po-detail1200.component';

describe('PoDetail1200Component', () => {
  let component: PoDetail1200Component;
  let fixture: ComponentFixture<PoDetail1200Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoDetail1200Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoDetail1200Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
