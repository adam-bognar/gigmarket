using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace GigMarket.Application.Features.SellerProfiles.Commands.ConnectStripeAccount;

public sealed class ConnectStripeAccountCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IConfiguration configuration)
    : IRequestHandler<ConnectStripeAccountCommand, ConnectStripeAccountResult>
{
    public async Task<ConnectStripeAccountResult> Handle(ConnectStripeAccountCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var seller = await db.SellerProfiles
            .FirstOrDefaultAsync(sp => sp.UserId == userId, ct)
            ?? throw new NotFoundException("Seller profile not found.");

        var clientBaseUrl = configuration["ClientBaseUrl"]
            ?? throw new InvalidOperationException("ClientBaseUrl is not configured.");

        var accountService = new AccountService();

        if (seller.StripeAccountId is null)
        {
            var account = await accountService.CreateAsync(new AccountCreateOptions
            {
                Type = "express",
                Metadata = new Dictionary<string, string>
                {
                    ["sellerProfileId"] = seller.Id.ToString(),
                    ["userId"] = userId.ToString()
                }
            }, cancellationToken: ct);

            seller.StripeAccountId = account.Id;
            seller.StripeAccountStatus = SellerStripeAccountStatus.Pending;
            await db.SaveChangesAsync(ct);
        }
        else
        {
            var account = await accountService.GetAsync(seller.StripeAccountId, cancellationToken: ct);
            if (account.ChargesEnabled && account.DetailsSubmitted)
            {
                if (seller.StripeAccountStatus == SellerStripeAccountStatus.Active)
                    return new ConnectStripeAccountResult(null, nameof(SellerStripeAccountStatus.Active));
                
                seller.StripeAccountStatus = SellerStripeAccountStatus.Active;
                await db.SaveChangesAsync(ct);
                return new ConnectStripeAccountResult(null, nameof(SellerStripeAccountStatus.Active));
            }
        }

        var linkService = new AccountLinkService();
        var link = await linkService.CreateAsync(new AccountLinkCreateOptions
        {
            Account = seller.StripeAccountId,
            RefreshUrl = $"{clientBaseUrl}/dashboard/seller/earnings?stripe_refresh=true",
            ReturnUrl  = $"{clientBaseUrl}/dashboard/seller/earnings?stripe_return=true",
            Type = "account_onboarding"
        }, cancellationToken: ct);

        return new ConnectStripeAccountResult(link.Url, seller.StripeAccountStatus.ToString());
    }
}