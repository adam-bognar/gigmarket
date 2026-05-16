using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Commands.FulfillOrder;
using MediatR;
using Stripe;
using Stripe.Checkout;

namespace GigMarket.Application.Features.Orders.Services;

public class StripeWebhookService(IMediator mediator) : IStripeWebhookService
{
    public async Task<StripeWebhookResult> HandleAsync(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        if (stripeEvent.Type != EventTypes.CheckoutSessionCompleted)
        {
            return StripeWebhookResult.Ok();
        }

        if (stripeEvent.Data.Object is not Session session)
        {
            return StripeWebhookResult.BadRequest("Invalid Stripe checkout session.");
        }

        var metadata = session.Metadata;

        if (!Guid.TryParse(metadata.GetValueOrDefault("gigId"), out var gigId) ||
            !Guid.TryParse(metadata.GetValueOrDefault("packageId"), out var packageId) ||
            !Guid.TryParse(metadata.GetValueOrDefault("buyerUserId"), out var buyerUserId))
        {
            return StripeWebhookResult.BadRequest("Missing or invalid metadata on Stripe session.");
        }

        var totalPrice = (session.AmountTotal ?? 0) / 100m;
        var paidAt = session.Created;

        await mediator.Send(new FulfillOrderCommand(
            session.Id,
            gigId,
            packageId,
            buyerUserId,
            totalPrice,
            paidAt), cancellationToken);

        return StripeWebhookResult.Ok();
    }
}