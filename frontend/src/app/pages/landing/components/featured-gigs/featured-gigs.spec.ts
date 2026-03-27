import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeaturedGigs } from './featured-gigs';

describe('FeaturedGigs', () => {
  let component: FeaturedGigs;
  let fixture: ComponentFixture<FeaturedGigs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeaturedGigs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeaturedGigs);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
