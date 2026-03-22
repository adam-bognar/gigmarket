import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GigDetails } from './gig-details';

describe('GigDetails', () => {
  let component: GigDetails;
  let fixture: ComponentFixture<GigDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GigDetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GigDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
