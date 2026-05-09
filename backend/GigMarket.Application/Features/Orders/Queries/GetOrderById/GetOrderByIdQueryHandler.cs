using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser, IBlobUrlResolverService blobUrlResolver)
    : IRequestHandler<GetOrderByIdQuery, OrderDetailDto>
{
    public async Task<OrderDetailDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Gig)
                .ThenInclude(g => g.SellerProfile)
                    .ThenInclude(sp => sp.User)
            .Include(o => o.Gig)
                .ThenInclude(g => g.Photos)
            .Include(o => o.Package)
            .Include(o => o.Buyer)
            .Include(o => o.Deliveries)
                .ThenInclude(d => d.Attachments)
            .Include(o => o.RevisionRequests)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct)
            ?? throw new NotFoundException($"Order '{request.Id}' not found.");

        var isBuyer  = order.BuyerUserId == userId;
        var isSeller = order.Gig.SellerProfile.UserId == userId;

        if (!isBuyer && !isSeller)
            throw new UnauthorizedException("You do not have access to this order.");

        var seller = order.Gig.SellerProfile;
        var primaryPhoto = order.Gig.Photos
            .Where(p => p.IsPrimary)
            .Select(p => p.Url)
            .FirstOrDefault() ?? string.Empty;
        
        var sellerImageUrl = blobUrlResolver.ResolveUrlAsync(seller.ProfileImageUrl, ct).Result;
        var primaryUrl = blobUrlResolver.ResolveUrlAsync(primaryPhoto, ct).Result;

        return new OrderDetailDto(
            order.Id,
            order.GigId,
            order.Gig.Title,
            primaryUrl,
            order.PackageId,
            order.Package.Name,
            order.Package.Tier.ToString(),
            order.Package.DeliveryDays,
            order.Package.Revisions,
            order.RevisionsUsed,
            order.TotalPrice,
            order.Status.ToString(),
            order.CreatedAtUtc,
            order.PaidAtUtc,
            order.DeadlineUtc,
            order.BuyerUserId,
            order.Buyer.CustomUsername,
            seller.UserId,
            seller.Id,
            seller.FirstName,
            seller.LastName,
            sellerImageUrl,
            order.Deliveries
                .OrderBy(d => d.CreatedAtUtc)
                .Select(d => new OrderDeliveryDto(
                    d.Id,
                    d.Message,
                    d.Attachments
                        .OrderBy(a => a.SortOrder)
                        .Select(a => a.FileUrl)
                        .ToList(),
                    d.CreatedAtUtc))
                .ToList(),
            order.RevisionRequests
                .OrderBy(r => r.CreatedAtUtc)
                .Select(r => new OrderRevisionRequestDto(r.Id, r.Message, r.CreatedAtUtc))
                .ToList()
        );
    }
}