namespace GigMarket.Application.Features.Messaging.Models;

public sealed record ConversationSummaryDto(
    Guid Id,
    Guid GigId,
    string GigTitle,
    string GigPrimaryPhotoUrl,
    Guid OtherUserId,
    string OtherUsername,
    string? OtherAvatarUrl,
    Guid? OrderId,
    string? OrderStatus,
    string? LastMessageContent,
    DateTime? LastMessageSentAt,
    int UnreadCount
);