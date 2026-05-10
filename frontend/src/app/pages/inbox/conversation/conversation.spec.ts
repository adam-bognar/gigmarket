import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConversationPage } from './conversation';

describe('ConversationPage', () => {
  let component: ConversationPage;
  let fixture: ComponentFixture<ConversationPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConversationPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConversationPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
