import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialCComponent } from './material-c.component';

describe('MaterialCComponent', () => {
  let component: MaterialCComponent;
  let fixture: ComponentFixture<MaterialCComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MaterialCComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
