using GigMarket.Application.Features.Messaging.Models;
using MediatR;

namespace GigMarket.Application.Features.Messaging.Commands.StartConversation;

public sealed record StartConversationCommand(Guid GigId, string InitialMessage) : IRequest<ConversationSummaryDto>;