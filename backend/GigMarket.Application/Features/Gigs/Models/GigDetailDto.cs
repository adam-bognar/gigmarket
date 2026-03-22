using GigMarket.Application.Features.Reviews.Models;

namespace GigMarket.Application.Features.Gigs.Models;

public sealed record GigDetailDto(
    Guid Id,
    string Title,
    string Description,
    string Status,
    DateTime CreatedAtUtc,

    Guid CategoryId,
    string CategoryName,
    Guid SubcategoryId,
    string SubcategoryName,

    Guid SellerProfileId,
    string SellerFirstName,
    string SellerLastName,
    string SellerAvatarUrl,

    string PrimaryPhotoUrl,
    List<string> AdditionalPhotoUrls,
    string? VideoUrl,

    List<string> Tags,

    List<GigDetailPackageDto> Packages,

    List<GigDetailRequirementDto> Requirements,

    double AverageRating,
    int TotalReviews,
    List<ReviewDto> Reviews
);

public sealed record GigDetailPackageDto(
    Guid Id,
    string Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price
);

public sealed record GigDetailRequirementDto(
    Guid Id,
    string Type,
    string Question,
    bool IsRequired,
    int SortOrder,
    List<string> Choices
);

