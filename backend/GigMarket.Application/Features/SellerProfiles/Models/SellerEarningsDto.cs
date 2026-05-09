namespace GigMarket.Application.Features.SellerProfiles.Models;

public sealed record SellerEarningsDto(
    decimal TotalEarned,
    decimal PendingEarnings,
    decimal PlatformFeesTotal,
    string StripeAccountStatus,
    List<EarningTransactionDto> Transactions
);

public sealed record EarningTransactionDto(
    Guid OrderId,
    string GigTitle,
    string BuyerUsername,
    DateTime CompletedAtUtc,
    decimal GrossAmount,
    decimal PlatformFee,
    decimal NetAmount,
    string PackageName,
    string PackageTier
);