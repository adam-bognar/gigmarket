import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryStrip } from './category-strip';

describe('CategoryStrip', () => {
  let component: CategoryStrip;
  let fixture: ComponentFixture<CategoryStrip>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryStrip]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoryStrip);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
