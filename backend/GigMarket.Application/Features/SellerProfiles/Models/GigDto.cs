namespace GigMarket.Application.Features.SellerProfiles.Models;

public sealed record GigDto(
    Guid Id,
    Guid SellerProfileId,
    string Title,
    string Category,
    string Subcategory,
    string Status,
    DateTime CreatedAtUtc,
    List<string> Tags,
    List<GigPackageDto> Packages,
    GigPhotosDto Photos
);

public sealed record GigPackageDto(
    Guid Id,
    string Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price
);

public sealed record GigPhotosDto(
    string PrimaryPhotoUrl,
    List<string> AdditionalPhotoUrls
);