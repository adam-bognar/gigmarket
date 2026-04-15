using GigMarket.Application.Features.Messaging.Models;
using MediatR;

namespace GigMarket.Application.Features.Messaging.Commands.MarkConversationRead;

public sealed record MarkConversationReadCommand(Guid ConversationId) : IRequest<MarkConversationReadResult>;