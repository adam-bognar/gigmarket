using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;

public sealed class CreateCheckoutSessionCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IConfiguration configuration,
    IStripeCheckoutService stripeCheckoutService)
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

        var primaryImageUrl = gig.Photos
            .Where(p => p.IsPrimary)
            .Select(p => p.Url)
            .FirstOrDefault();

        return await stripeCheckoutService.CreateCheckoutSessionAsync(
            gigTitle: gig.Title,
            packageName: package.Name,
            packageDescription: package.Description,
            primaryImageUrl: primaryImageUrl,
            price: package.Price,
            gigId: gig.Id,
            packageId: package.Id,
            buyerUserId: buyerUserId,
            clientBaseUrl: clientBaseUrl,
            cancellationToken: ct);
    }
}