using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Messaging.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Messaging.Commands.MarkConversationRead;

public sealed class MarkConversationReadCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<MarkConversationReadCommand, MarkConversationReadResult>
{
    public async Task<MarkConversationReadResult> Handle(MarkConversationReadCommand command, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId.Value;

        var conversation = await db.Conversations
            .FirstOrDefaultAsync(c => c.Id == command.ConversationId, ct)
            ?? throw new NotFoundException($"Conversation '{command.ConversationId}' was not found.");

        if (conversation.BuyerUserId != userId && conversation.SellerUserId != userId)
            throw new UnauthorizedException("You are not a participant in this conversation.");

        var unreadMessages = await db.Messages
            .Where(m => m.ConversationId == command.ConversationId
                        && m.SenderUserId != userId
                        && !m.IsRead)
            .ToListAsync(ct);

        if (unreadMessages.Count == 0)
            return new MarkConversationReadResult(command.ConversationId, userId);

        foreach (var msg in unreadMessages)
            msg.IsRead = true;

        await db.SaveChangesAsync(ct);

        var otherUserId = conversation.BuyerUserId == userId
            ? conversation.SellerUserId
            : conversation.BuyerUserId;

        return new MarkConversationReadResult(command.ConversationId, otherUserId);
    }
}