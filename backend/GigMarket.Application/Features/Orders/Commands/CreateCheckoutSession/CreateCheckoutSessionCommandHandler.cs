using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;

public sealed class CreateCheckoutSessionCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IConfiguration configuration)
    : IRequestHandler<CreateCheckoutSessionCommand, string>
{
    public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");
 
        var buyerUserId = currentUser.UserId!.Value;
 
        var package = await db.GigPackages
            .Include(p => p.Gig)
                .ThenInclude(g => g.Photos)
            .FirstOrDefaultAsync(p => p.Id == request.PackageId && p.GigId == request.GigId, ct)
            ?? throw new NotFoundException($"Package '{request.PackageId}' not found on gig '{request.GigId}'.");
 
        var gig = package.Gig;
 
        var sellerUserId = await db.SellerProfiles
            .Where(sp => sp.Id == gig.SellerProfileId)
            .Select(sp => sp.UserId)
            .FirstOrDefaultAsync(ct);
 
        if (sellerUserId == buyerUserId)
            throw new BadRequestException("You cannot purchase your own gig.");
 
        var clientBaseUrl = configuration["ClientBaseUrl"]
            ?? throw new InvalidOperationException("ClientBaseUrl is not configured.");
 
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(package.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{gig.Title} — {package.Name}",
                            Description = package.Description,
                            Images = gig.Photos
                                .Where(p => p.IsPrimary)
                                .Select(p => p.Url)
                                .Take(1)
                                .ToList()
                        }
                    },
                    Quantity = 1
                }
            ],
            Mode = "payment",
            SuccessUrl = $"{clientBaseUrl}/orders/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{clientBaseUrl}/gigs/{gig.Id}",
            Metadata = new Dictionary<string, string>
            {
                ["gigId"]      = gig.Id.ToString(),
                ["packageId"]  = package.Id.ToString(),
                ["buyerUserId"] = buyerUserId.ToString()
            }
        };
 
        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);
 
        return session.Url;
    }
}