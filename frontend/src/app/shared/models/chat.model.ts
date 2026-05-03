export interface MessageDto {
  id: string;
  conversationId: string;
  senderUserId: string;
  senderUsername: string;
  senderAvatarUrl: string | null;
  content: string;
  sentAtUtc: string;
  isRead: boolean;
}

export interface ConversationSummaryDto {
  id: string;
  gigId: string;
  gigTitle: string;
  gigPrimaryPhotoUrl: string;
  otherUserId: string;
  otherUsername: string;
  otherAvatarUrl: string | null;
  orderId: string | null;
  orderStatus: string | null;
  lastMessageContent: string | null;
  lastMessageSentAt: string | null;
  unreadCount: number;
}

export interface StartConversationPayload {
  gigId: string;
}

export interface SendMessagePayload {
  content: string;
}
