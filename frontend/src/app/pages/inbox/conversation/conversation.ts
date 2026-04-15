import {
  AfterViewChecked,
  ChangeDetectionStrategy,
  Component,
  computed,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Send, ArrowLeft } from 'lucide-angular';
import { RouterLink } from '@angular/router';
import {ChatService} from '../../../shared/services/chat.service';
import {AuthService} from '../../../shared/services/auth.service';

@Component({
  selector: 'app-conversation',
  imports: [DatePipe, FormsModule, LucideAngularModule, RouterLink],
  templateUrl: './conversation.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConversationPage implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messageList') private messageListRef!: ElementRef<HTMLDivElement>;

  private readonly route = inject(ActivatedRoute);
  private readonly chatService = inject(ChatService);
  private readonly authService = inject(AuthService);

  protected readonly SendIcon = Send;
  protected readonly ArrowLeftIcon = ArrowLeft;

  protected readonly messages = this.chatService.activeMessages;
  protected readonly isTyping = this.chatService.isTyping;
  protected readonly conversations = this.chatService.conversations;
  protected readonly currentUser = this.authService.user;

  protected readonly messageContent = signal('');
  protected readonly isSending = signal(false);
  protected readonly conversationId = signal('');

  protected readonly conversation = computed(() =>
    this.conversations().find((c) => c.id === this.conversationId())
  );

  private typingTimer: ReturnType<typeof setTimeout> | null = null;
  private shouldScrollToBottom = false;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('conversationId') ?? '';
    this.conversationId.set(id);
    this.chatService.clearActiveMessages();

    this.chatService.loadMessages(id).subscribe({
      next: () => {
        this.shouldScrollToBottom = true;
      },
      error: (err) => console.error('[ConversationPage] Failed to load messages:', err),
    });

    this.chatService.markRead(id).subscribe();
    this.chatService.joinConversation(id);
  }

  ngOnDestroy(): void {
    this.chatService.leaveConversation(this.conversationId());
    this.clearTypingTimer();
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  protected onInput(): void {
    this.chatService.sendTyping(this.conversationId(), true);
    this.clearTypingTimer();
    this.typingTimer = setTimeout(() => {
      this.chatService.sendTyping(this.conversationId(), false);
    }, 2000);
  }

  protected sendMessage(): void {
    const content = this.messageContent().trim();
    if (!content || this.isSending()) return;

    this.isSending.set(true);
    this.chatService.sendMessage(this.conversationId(), { content }).subscribe({
      next: () => {
        this.messageContent.set('');
        this.isSending.set(false);
        this.shouldScrollToBottom = true;
      },
      error: (err) => {
        console.error('[ConversationPage] Failed to send message:', err);
        this.isSending.set(false);
      },
    });
  }

  protected onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  protected isMine(senderUserId: string): boolean {
    return this.currentUser()?.id === senderUserId;
  }

  private scrollToBottom(): void {
    const el = this.messageListRef?.nativeElement;
    if (el) el.scrollTop = el.scrollHeight;
  }

  private clearTypingTimer(): void {
    if (this.typingTimer !== null) {
      clearTimeout(this.typingTimer);
      this.typingTimer = null;
    }
  }
}
