using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetStripeDashboardLink;

public sealed class GetStripeDashboardLinkQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetStripeDashboardLinkQuery, string>
{
    public async Task<string> Handle(GetStripeDashboardLinkQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var seller = await db.SellerProfiles
                         .FirstOrDefaultAsync(sp => sp.UserId == userId, ct)
                     ?? throw new NotFoundException("Seller profile not found.");

        if (seller.StripeAccountId is null || seller.StripeAccountStatus != SellerStripeAccountStatus.Active)
            throw new BadRequestException("Stripe account is not fully connected yet.");

        var client = new StripeClient(StripeConfiguration.ApiKey);
        var loginLink = await client.V1.Accounts.LoginLinks.CreateAsync(
            seller.StripeAccountId,
            new AccountLoginLinkCreateOptions(),
            cancellationToken: ct);

        return loginLink.Url;
    }
}