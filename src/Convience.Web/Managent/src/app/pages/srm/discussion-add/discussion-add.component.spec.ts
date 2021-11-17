import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscussionAddComponent } from './discussion-add.component';

describe('DiscussionAddComponent', () => {
  let component: DiscussionAddComponent;
  let fixture: ComponentFixture<DiscussionAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiscussionAddComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DiscussionAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
