namespace GigMarket.Application.Common.Interfaces;

public interface IStripeCheckoutService
{
    Task<string> CreateCheckoutSessionAsync(
        string gigTitle,
        string packageName,
        string packageDescription,
        string? primaryImageUrl,
        decimal price,
        Guid gigId,
        Guid packageId,
        Guid buyerUserId,
        string clientBaseUrl,
        CancellationToken cancellationToken);
}