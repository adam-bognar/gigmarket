using GigMarket.Application.Common.Interfaces;
using Stripe.Checkout;

namespace GigMarket.Infrastructure.Service;

public class StripeCheckoutService(IStripeSessionService stripeSessionService) : IStripeCheckoutService
{
    public async Task<string> CreateCheckoutSessionAsync(
        string gigTitle,
        string packageName,
        string packageDescription,
        string? primaryImageUrl,
        decimal price,
        Guid gigId,
        Guid packageId,
        Guid buyerUserId,
        string clientBaseUrl,
        CancellationToken cancellationToken)
    {
        var productData = new SessionLineItemPriceDataProductDataOptions
        {
            Name = $"{gigTitle} — {packageName}",
            Description = packageDescription
        };

        if (!string.IsNullOrWhiteSpace(primaryImageUrl))
        {
            productData.Images = [primaryImageUrl];
        }

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
                        UnitAmount = (long)(price * 100),
                        ProductData = productData
                    },
                    Quantity = 1
                }
            ],
            Mode = "payment",
            SuccessUrl = $"{clientBaseUrl}/orders/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{clientBaseUrl}/gigs/{gigId}",
            Metadata = new Dictionary<string, string>
            {
                ["gigId"] = gigId.ToString(),
                ["packageId"] = packageId.ToString(),
                ["buyerUserId"] = buyerUserId.ToString()
            }
        };

        var session = await stripeSessionService.CreateAsync(options, cancellationToken);

        return session.Url;
    }
}