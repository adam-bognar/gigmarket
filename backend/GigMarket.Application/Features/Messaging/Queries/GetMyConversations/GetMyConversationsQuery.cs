using GigMarket.Application.Features.Messaging.Models;
using MediatR;

namespace GigMarket.Application.Features.Messaging.Queries.GetMyConversations;

public sealed record GetMyConversationsQuery : IRequest<List<ConversationSummaryDto>>;