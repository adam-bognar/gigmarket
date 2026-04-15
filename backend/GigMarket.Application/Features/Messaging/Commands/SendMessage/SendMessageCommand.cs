using GigMarket.Application.Features.Messaging.Models;
using MediatR;

namespace GigMarket.Application.Features.Messaging.Commands.SendMessage;

public sealed record SendMessageCommand(Guid ConversationId, string Content) : IRequest<SendMessageResult>;