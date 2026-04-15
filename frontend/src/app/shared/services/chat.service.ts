import { computed, inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConversationSummaryDto, MessageDto, SendMessagePayload, StartConversationPayload } from '../models/chat.model';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly baseUrl = `${environment.apiUrl}/conversations`;
  private readonly hubUrl = `${environment.apiUrl.replace('/api', '')}/hubs/chat`;

  private hubConnection: HubConnection | null = null;

  readonly conversations = signal<ConversationSummaryDto[]>([]);
  readonly activeMessages = signal<MessageDto[]>([]);
  readonly unreadCount = signal<number>(0);
  private readonly isConnected = signal<boolean>(false);
  private readonly typingUsers = signal<Set<string>>(new Set());
  readonly isTyping = computed(() => this.typingUsers().size > 0);

  startConnection(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.hubConnection?.state === HubConnectionState.Connected) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, { withCredentials: true })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.registerHandlers();

    this.hubConnection
      .start()
      .then(() => {
        this.isConnected.set(true);
        console.log('[ChatService] SignalR connected');
      })
      .catch((err) => console.error('[ChatService] SignalR connection error:', err));
  }

  stopConnection(): void {
    this.hubConnection?.stop().then(() => this.isConnected.set(false));
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('MessageReceived', (message: MessageDto) => {
      this.activeMessages.update((msgs) => [...msgs, message]);
      this.conversations.update((convs) =>
        convs.map((c) =>
          c.id === message.conversationId
            ? { ...c, lastMessageContent: message.content, lastMessageSentAt: message.sentAtUtc, unreadCount: c.unreadCount + 1 }
            : c
        )
      );
      this.unreadCount.update((n) => n + 1);
    });

    this.hubConnection.on('ConversationStarted', (conversation: ConversationSummaryDto) => {
      this.conversations.update((convs) => [conversation, ...convs]);
      this.unreadCount.update((n) => n + 1);
    });

    this.hubConnection.on('ReadReceipt', (conversationId: string) => {
      this.activeMessages.update((msgs) =>
        msgs.map((m) => (m.conversationId === conversationId ? { ...m, isRead: true } : m))
      );
    });

    this.hubConnection.on('TypingIndicator', (_conversationId: string, userId: string, isTyping: boolean) => {
      this.typingUsers.update((users) => {
        const next = new Set(users);
        isTyping ? next.add(userId) : next.delete(userId);
        return next;
      });
    });
  }

  joinConversation(conversationId: string): void {
    this.hubConnection?.invoke('JoinConversation', conversationId).catch(console.error);
  }

  leaveConversation(conversationId: string): void {
    this.hubConnection?.invoke('LeaveConversation', conversationId).catch(console.error);
  }

  sendTyping(conversationId: string, isTyping: boolean): void {
    this.hubConnection?.invoke('SendTyping', conversationId, isTyping).catch(console.error);
  }

  startConversation(payload: StartConversationPayload): Observable<ConversationSummaryDto> {
    return this.http.post<ConversationSummaryDto>(this.baseUrl, payload, { withCredentials: true }).pipe(
      tap((conv) => {
        const exists = this.conversations().some((c) => c.id === conv.id);
        if (!exists) this.conversations.update((convs) => [conv, ...convs]);
      })
    );
  }

  loadConversations(): Observable<ConversationSummaryDto[]> {
    return this.http.get<ConversationSummaryDto[]>(this.baseUrl, { withCredentials: true }).pipe(
      tap((convs) => this.conversations.set(convs))
    );
  }

  loadMessages(conversationId: string, page = 1): Observable<MessageDto[]> {
    return this.http
      .get<MessageDto[]>(`${this.baseUrl}/${conversationId}/messages`, {
        params: { page, pageSize: 30 },
        withCredentials: true,
      })
      .pipe(tap((msgs) => this.activeMessages.set(msgs)));
  }

  sendMessage(conversationId: string, payload: SendMessagePayload): Observable<MessageDto> {
    return this.http
      .post<MessageDto>(`${this.baseUrl}/${conversationId}/messages`, payload, { withCredentials: true })
      .pipe(
        tap((msg) => {
          this.activeMessages.update((msgs) => [...msgs, msg]);
          this.conversations.update((convs) =>
            convs.map((c) =>
              c.id === conversationId
                ? { ...c, lastMessageContent: msg.content, lastMessageSentAt: msg.sentAtUtc }
                : c
            )
          );
        })
      );
  }

  markRead(conversationId: string): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}/${conversationId}/read`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          this.conversations.update((convs) =>
            convs.map((c) => (c.id === conversationId ? { ...c, unreadCount: 0 } : c))
          );
          this.unreadCount.update((n) => Math.max(0, n - 1));
        })
      );
  }

  loadUnreadCount(): Observable<{ count: number }> {
    return this.http
      .get<{ count: number }>(`${this.baseUrl}/unread-count`, { withCredentials: true })
      .pipe(tap(({ count }) => this.unreadCount.set(count)));
  }

  clearActiveMessages(): void {
    this.activeMessages.set([]);
  }
}
