using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Messaging.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Messaging.Commands.SendMessage;

public sealed class SendMessageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    public async Task<SendMessageResult> Handle(SendMessageCommand command, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var senderUserId = currentUser.UserId.Value;

        var conversation = await db.Conversations
            .FirstOrDefaultAsync(c => c.Id == command.ConversationId, ct)
            ?? throw new NotFoundException($"Conversation '{command.ConversationId}' was not found.");

        if (conversation.BuyerUserId != senderUserId && conversation.SellerUserId != senderUserId)
            throw new UnauthorizedException("You are not a participant in this conversation.");

        var sender = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == senderUserId, ct)
            ?? throw new NotFoundException("Sender not found.");

        var recipientUserId = conversation.BuyerUserId == senderUserId
            ? conversation.SellerUserId
            : conversation.BuyerUserId;

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderUserId = senderUserId,
            Content = command.Content,
            SentAtUtc = DateTime.UtcNow,
            IsRead = false
        };

        db.Messages.Add(message);
        conversation.LastMessageAtUtc = message.SentAtUtc;

        await db.SaveChangesAsync(ct);

        var dto = new MessageDto(
            Id: message.Id,
            ConversationId: message.ConversationId,
            SenderUserId: message.SenderUserId,
            SenderUsername: sender.CustomUsername,
            SenderAvatarUrl: null,
            Content: message.Content,
            SentAtUtc: message.SentAtUtc,
            IsRead: message.IsRead
        );

        return new SendMessageResult(dto, recipientUserId);
    }
}