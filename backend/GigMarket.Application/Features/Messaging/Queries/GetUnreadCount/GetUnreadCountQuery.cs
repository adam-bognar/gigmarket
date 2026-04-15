using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Messaging.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery : IRequest<int>;

public sealed class GetUnreadCountQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetUnreadCountQuery, int>
{
    public async Task<int> Handle(GetUnreadCountQuery query, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId.Value;

        return await db.Conversations
            .AsNoTracking()
            .Where(c => c.BuyerUserId == userId || c.SellerUserId == userId)
            .CountAsync(c => c.Messages.Any(m => !m.IsRead && m.SenderUserId != userId), ct);
    }
}