using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Orders.Queries.GetMySellerOrders;

public sealed class GetMySellerOrdersQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMySellerOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetMySellerOrdersQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var sellerUserId = currentUser.UserId!.Value;

        return await db.Orders
            .AsNoTracking()
            .Where(o => o.Gig.SellerProfile.UserId == sellerUserId)
            .Include(o => o.Gig).ThenInclude(g => g.Photos)
            .Include(o => o.Package)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderDto(
                o.Id,
                o.GigId,
                o.Gig.Title,
                o.Gig.Photos.Where(p => p.IsPrimary).Select(p => p.Url).FirstOrDefault() ?? string.Empty,
                o.PackageId,
                o.Package.Name,
                o.Package.Tier.ToString(),
                o.Package.DeliveryDays,
                o.TotalPrice,
                o.Status.ToString(),
                o.CreatedAtUtc,
                o.PaidAtUtc
            ))
            .ToListAsync(ct);
    }
}