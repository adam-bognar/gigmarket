namespace GigMarket.Application.Features.Orders.Models;

public sealed record OrderDto(
    Guid Id,
    Guid GigId,
    string GigTitle,
    string GigPrimaryPhotoUrl,
    Guid PackageId,
    string PackageName,
    string PackageTier,
    int DeliveryDays,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc
);