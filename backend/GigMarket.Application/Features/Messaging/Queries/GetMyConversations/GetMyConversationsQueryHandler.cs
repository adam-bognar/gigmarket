using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Messaging.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Messaging.Queries.GetMyConversations;

public sealed class GetMyConversationsQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetMyConversationsQuery, List<ConversationSummaryDto>>
{
    public async Task<List<ConversationSummaryDto>> Handle(GetMyConversationsQuery query, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId.Value;

        var conversations = await db.Conversations
            .AsNoTracking()
            .Where(c => c.BuyerUserId == userId || c.SellerUserId == userId)
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Gig).ThenInclude(g => g.Photos)
            .Include(c => c.Messages)
            .OrderByDescending(c => c.LastMessageAtUtc)
            .ToListAsync(ct);

        return conversations.Select(c =>
        {
            var isBuyer = c.BuyerUserId == userId;
            var other = isBuyer ? c.Seller : c.Buyer;

            var lastMsg = c.Messages
                .OrderByDescending(m => m.SentAtUtc)
                .FirstOrDefault();

            var unreadCount = c.Messages
                .Count(m => !m.IsRead && m.SenderUserId != userId);

            var primaryPhoto = c.Gig.Photos
                .FirstOrDefault(p => p.IsPrimary)?.Url ?? string.Empty;

            return new ConversationSummaryDto(
                Id: c.Id,
                GigId: c.GigId,
                GigTitle: c.Gig.Title,
                GigPrimaryPhotoUrl: primaryPhoto,
                OtherUserId: other.Id,
                OtherUsername: other.CustomUsername,
                OtherAvatarUrl: null,
                OrderId: c.OrderId,
                OrderStatus: null,
                LastMessageContent: lastMsg?.Content,
                LastMessageSentAt: lastMsg?.SentAtUtc,
                UnreadCount: unreadCount
            );
        }).ToList();
    }
}