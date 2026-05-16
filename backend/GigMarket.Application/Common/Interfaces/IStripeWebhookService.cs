using Stripe;

namespace GigMarket.Application.Common.Interfaces;

public interface IStripeWebhookService
{
    Task<StripeWebhookResult> HandleAsync(Event stripeEvent, CancellationToken cancellationToken);
}

public sealed record StripeWebhookResult(
    bool Success,
    string? Error = null)
{
    public static StripeWebhookResult Ok() => new(true);

    public static StripeWebhookResult BadRequest(string error) => new(false, error);
}