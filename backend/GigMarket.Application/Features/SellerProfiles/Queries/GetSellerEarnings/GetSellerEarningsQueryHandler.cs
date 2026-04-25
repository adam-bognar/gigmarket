using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetSellerEarnings;

public sealed class GetSellerEarningsQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetSellerEarningsQuery, SellerEarningsDto>
{
    private const decimal PlatformFeePercent = 0.20m;

    public async Task<SellerEarningsDto> Handle(GetSellerEarningsQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var seller = await db.SellerProfiles
            .FirstOrDefaultAsync(sp => sp.UserId == userId, ct)
            ?? throw new NotFoundException("Seller profile not found.");

        if (seller.StripeAccountId is not null && seller.StripeAccountStatus == SellerStripeAccountStatus.Pending)
        {
            var accountService = new AccountService();
            var account = await accountService.GetAsync(seller.StripeAccountId, cancellationToken: ct);
            if (account.ChargesEnabled && account.DetailsSubmitted)
            {
                seller.StripeAccountStatus = SellerStripeAccountStatus.Active;
                await db.SaveChangesAsync(ct);
            }
        }

        var orders = await db.Orders
            .Include(o => o.Gig)
            .Include(o => o.Package)
            .Include(o => o.Buyer)
            .Where(o => o.Gig.SellerProfileId == seller.Id)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(ct);

        var completed = orders.Where(o => o.Status == OrderStatus.Completed).ToList();
        var inProgress = orders.Where(o =>
            o.Status is OrderStatus.InProgress or OrderStatus.Delivered or OrderStatus.UnderRevision).ToList();

        var totalEarned = completed.Sum(o => o.TotalPrice * (1 - PlatformFeePercent));
        var platformFeesTotal = completed.Sum(o => o.TotalPrice * PlatformFeePercent);
        var pendingEarnings = inProgress.Sum(o => o.TotalPrice * (1 - PlatformFeePercent));

        var transactions = completed.Select(o => new EarningTransactionDto(
            o.Id,
            o.Gig.Title,
            o.Buyer.UserName ?? o.Buyer.Email ?? "Unknown",
            o.PaidAtUtc ?? o.CreatedAtUtc,
            o.TotalPrice,
            Math.Round(o.TotalPrice * PlatformFeePercent, 2),
            Math.Round(o.TotalPrice * (1 - PlatformFeePercent), 2),
            o.Package.Name,
            o.Package.Tier.ToString()
        )).ToList();

        return new SellerEarningsDto(
            Math.Round(totalEarned, 2),
            Math.Round(pendingEarnings, 2),
            Math.Round(platformFeesTotal, 2),
            seller.StripeAccountStatus.ToString(),
            transactions
        );
    }
}