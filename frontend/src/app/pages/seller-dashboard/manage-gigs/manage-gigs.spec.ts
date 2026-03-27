import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageGigs } from './manage-gigs';

describe('ManageGigs', () => {
  let component: ManageGigs;
  let fixture: ComponentFixture<ManageGigs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageGigs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageGigs);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
