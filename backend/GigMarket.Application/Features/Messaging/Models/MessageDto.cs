namespace GigMarket.Application.Features.Messaging.Models;

public sealed record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderUserId,
    string SenderUsername,
    string? SenderAvatarUrl,
    string Content,
    DateTime SentAtUtc,
    bool IsRead
);