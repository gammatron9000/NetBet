import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BetDisplayComponent } from './bet-display.component';

describe('BetDisplayComponent', () => {
  let component: BetDisplayComponent;
  let fixture: ComponentFixture<BetDisplayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BetDisplayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BetDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
