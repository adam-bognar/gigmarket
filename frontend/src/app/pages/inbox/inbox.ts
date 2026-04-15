import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { LucideAngularModule, MessageSquare, Inbox } from 'lucide-angular';
import {ChatService} from '../../shared/services/chat.service';

@Component({
  selector: 'app-inbox',
  imports: [RouterLink, DatePipe, LucideAngularModule],
  templateUrl: './inbox.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InboxPage implements OnInit {
  protected readonly chatService = inject(ChatService);
  protected readonly MessageSquareIcon = MessageSquare;
  protected readonly InboxIcon = Inbox;

  protected readonly conversations = this.chatService.conversations;
  protected readonly isLoading = false;

  ngOnInit(): void {
    this.chatService.loadConversations().subscribe({
      error: (err) => console.error('[InboxPage] Failed to load conversations:', err),
    });
  }
}
