using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = GigMarket.Domain.Entities.Order;

namespace GigMarket.Application.Features.Orders.Commands.AcceptDelivery;

public sealed class AcceptDeliveryCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    ILogger<AcceptDeliveryCommandHandler> logger)
    : IRequestHandler<AcceptDeliveryCommand>
{
    private const decimal PlatformFeePercent = 0.20m;

    public async Task Handle(AcceptDeliveryCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var order = await db.Orders
            .Include(o => o.Gig)
                .ThenInclude(g => g.SellerProfile)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
            ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        if (order.BuyerUserId != userId)
            throw new UnauthorizedException("Only the buyer can accept a delivery.");

        if (order.Status != OrderStatus.Delivered)
            throw new BadRequestException($"Cannot accept a delivery on an order with status '{order.Status}'.");

        order.Status = OrderStatus.Completed;
        await db.SaveChangesAsync(ct);

        await TryTransferToSellerAsync(order, ct);
    }

    private async Task TryTransferToSellerAsync(Order order, CancellationToken ct)
    {
        var seller = order.Gig.SellerProfile;

        if (seller.StripeAccountId is null || seller.StripeAccountStatus != SellerStripeAccountStatus.Active)
        {
            logger.LogWarning(
                "[AcceptDelivery] Skipping Stripe transfer for order {OrderId} — seller has no active Stripe account.",
                order.Id);
            return;
        }

        var transferAmount = (long)(order.TotalPrice * (1 - PlatformFeePercent) * 100);

        try
        {
            var transferService = new TransferService();
            await transferService.CreateAsync(new TransferCreateOptions
            {
                Amount      = transferAmount,
                Currency    = "usd",
                Destination = seller.StripeAccountId,
                Metadata    = new Dictionary<string, string>
                {
                    ["orderId"]          = order.Id.ToString(),
                    ["sellerProfileId"]  = seller.Id.ToString()
                }
            }, cancellationToken: ct);

            logger.LogInformation(
                "[AcceptDelivery] Transferred ${Amount} to seller {StripeAccountId} for order {OrderId}.",
                transferAmount / 100m, seller.StripeAccountId, order.Id);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex,
                "[AcceptDelivery] Stripe transfer failed for order {OrderId}. Manual payout may be required.",
                order.Id);
        }
    }
}