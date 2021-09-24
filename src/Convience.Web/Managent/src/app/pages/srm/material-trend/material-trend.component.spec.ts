import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialTrendComponent } from './material-trend.component';

describe('MaterialTrendComponent', () => {
  let component: MaterialTrendComponent;
  let fixture: ComponentFixture<MaterialTrendComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MaterialTrendComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialTrendComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
