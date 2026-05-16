using Stripe.Checkout;

namespace GigMarket.Application.Common.Interfaces;

public interface IStripeSessionService
{
    Task<Session> CreateAsync(
        SessionCreateOptions options,
        CancellationToken cancellationToken);
}