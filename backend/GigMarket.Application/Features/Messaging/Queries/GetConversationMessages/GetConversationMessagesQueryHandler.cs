using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Messaging.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Messaging.Queries.GetConversationMessages;

public sealed class GetConversationMessagesQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetConversationMessagesQuery, List<MessageDto>>
{
    public async Task<List<MessageDto>> Handle(GetConversationMessagesQuery query, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId.Value;

        var conversation = await db.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.ConversationId, ct)
            ?? throw new NotFoundException($"Conversation '{query.ConversationId}' was not found.");

        if (conversation.BuyerUserId != userId && conversation.SellerUserId != userId)
            throw new UnauthorizedException("You are not a participant in this conversation.");

        var messages = await db.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == query.ConversationId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.SentAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return messages
            .OrderBy(m => m.SentAtUtc)
            .Select(m => new MessageDto(
                Id: m.Id,
                ConversationId: m.ConversationId,
                SenderUserId: m.SenderUserId,
                SenderUsername: m.Sender.CustomUsername,
                SenderAvatarUrl: null,
                Content: m.Content,
                SentAtUtc: m.SentAtUtc,
                IsRead: m.IsRead
            ))
            .ToList();
    }
}