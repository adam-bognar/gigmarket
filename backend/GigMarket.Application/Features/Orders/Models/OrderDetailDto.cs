namespace GigMarket.Application.Features.Orders.Models;

public sealed record OrderDetailDto(
    Guid Id,
    Guid GigId,
    string GigTitle,
    string GigPrimaryPhotoUrl,
    Guid PackageId,
    string PackageName,
    string PackageTier,
    int DeliveryDays,
    int RevisionsAllowed,
    int RevisionsUsed,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc,
    DateTime? DeadlineUtc,
    Guid BuyerUserId,
    string BuyerUsername,
    Guid SellerUserId,
    Guid SellerProfileId,
    string SellerFirstName,
    string SellerLastName,
    string SellerAvatarUrl,
    List<OrderDeliveryDto> Deliveries,
    List<OrderRevisionRequestDto> RevisionRequests
);

public sealed record OrderDeliveryDto(
    Guid Id,
    string Message,
    List<string> FileUrls,
    DateTime CreatedAtUtc
);

public sealed record OrderRevisionRequestDto(
    Guid Id,
    string Message,
    DateTime CreatedAtUtc
);