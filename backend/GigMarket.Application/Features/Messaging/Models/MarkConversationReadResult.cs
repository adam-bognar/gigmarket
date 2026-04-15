namespace GigMarket.Application.Features.Messaging.Models;

public sealed record MarkConversationReadResult(Guid ConversationId, Guid SenderUserId);