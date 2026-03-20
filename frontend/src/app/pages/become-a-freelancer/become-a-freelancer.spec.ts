import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BecomeAFreelancer } from './become-a-freelancer';

describe('BecomeAFreelancer', () => {
  let component: BecomeAFreelancer;
  let fixture: ComponentFixture<BecomeAFreelancer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BecomeAFreelancer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BecomeAFreelancer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
