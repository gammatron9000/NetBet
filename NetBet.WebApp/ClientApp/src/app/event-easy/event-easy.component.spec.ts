import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EventEasyComponent } from './event-easy.component';

describe('EventEasyComponent', () => {
  let component: EventEasyComponent;
  let fixture: ComponentFixture<EventEasyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EventEasyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EventEasyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
