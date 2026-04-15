using GigMarket.Application.Features.Messaging.Models;
using MediatR;

namespace GigMarket.Application.Features.Messaging.Queries.GetConversationMessages;

public sealed record GetConversationMessagesQuery(
    Guid ConversationId,
    int Page = 1,
    int PageSize = 30
) : IRequest<List<MessageDto>>;