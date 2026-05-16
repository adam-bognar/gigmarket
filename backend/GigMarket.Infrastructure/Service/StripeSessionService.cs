using GigMarket.Application.Common.Interfaces;
using Stripe.Checkout;

namespace GigMarket.Infrastructure.Service;

public class StripeSessionService : IStripeSessionService
{
    private readonly SessionService _sessionService = new();

    public async Task<Session> CreateAsync(
        SessionCreateOptions options,
        CancellationToken cancellationToken)
    {
        return await _sessionService.CreateAsync(
            options,
            cancellationToken: cancellationToken);
    }
}