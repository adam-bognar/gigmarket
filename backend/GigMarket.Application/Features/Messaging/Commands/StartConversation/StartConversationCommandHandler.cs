using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Messaging.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Messaging.Commands.StartConversation;

public sealed class StartConversationCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser, IBlobUrlResolverService blobUrlResolver)
    : IRequestHandler<StartConversationCommand, ConversationSummaryDto>
{
    public async Task<ConversationSummaryDto> Handle(StartConversationCommand command, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var buyerUserId = currentUser.UserId.Value;

        var gig = await db.Gigs
            .Include(g => g.SellerProfile)
            .Include(g => g.Photos)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == command.GigId, ct)
            ?? throw new NotFoundException($"Gig '{command.GigId}' was not found.");

        var sellerUserId = gig.SellerProfile.UserId;

        if (buyerUserId == sellerUserId)
            throw new BadRequestException("You cannot start a conversation with yourself.");

        var conversation = await db.Conversations
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c =>
                c.BuyerUserId == buyerUserId &&
                c.SellerUserId == sellerUserId &&
                c.GigId == command.GigId, ct);

        string sellerUsername;
        string? lastMessageContent = null;
        DateTime? lastMessageSentAt = null;
        int unreadCount = 0;

        if (conversation is null)
        {
            sellerUsername = await db.Users
                .Where(u => u.Id == sellerUserId)
                .Select(u => u.CustomUsername)
                .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("Seller user not found.");

            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                BuyerUserId = buyerUserId,
                SellerUserId = sellerUserId,
                GigId = command.GigId,
                CreatedAtUtc = DateTime.UtcNow,
                LastMessageAtUtc = DateTime.UtcNow
            };

            db.Conversations.Add(conversation);
            await db.SaveChangesAsync(ct);
        }
        else
        {
            sellerUsername = conversation.Seller.CustomUsername;
            var last = conversation.Messages.FirstOrDefault();
            lastMessageContent = last?.Content;
            lastMessageSentAt = last?.SentAtUtc;
            unreadCount = conversation.Messages.Count(m => !m.IsRead && m.SenderUserId != buyerUserId);
        }

        var primaryPhoto = blobUrlResolver.ResolveUrlAsync(gig.Photos.FirstOrDefault(p => p.IsPrimary)?.Url ?? string.Empty, ct).Result;

        return new ConversationSummaryDto(
            Id: conversation.Id,
            GigId: gig.Id,
            GigTitle: gig.Title,
            GigPrimaryPhotoUrl: primaryPhoto,
            OtherUserId: sellerUserId,
            OtherUsername: sellerUsername,
            OtherAvatarUrl: null,
            OrderId: conversation.OrderId,
            OrderStatus: null,
            LastMessageContent: lastMessageContent,
            LastMessageSentAt: lastMessageSentAt,
            UnreadCount: unreadCount
        );
    }
}