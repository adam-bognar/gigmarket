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
)
{
    public GigDetailDto() : this(
        Guid.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        DateTime.UtcNow,
        Guid.Empty,
        string.Empty,
        Guid.Empty,
        string.Empty,
        Guid.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new List<string>(),
        null,
        new List<string>(),
        new List<GigDetailPackageDto>(),
        new List<GigDetailRequirementDto>(),
        0d,
        0,
        new List<ReviewDto>())
    {
    }
}

public sealed record GigDetailPackageDto(
    Guid Id,
    string Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price
)
{
    public GigDetailPackageDto() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0m)
    {
    }
}

public sealed record GigDetailRequirementDto(
    Guid Id,
    string Type,
    string Question,
    bool IsRequired,
    int SortOrder,
    List<string> Choices
)
{
    public GigDetailRequirementDto() : this(Guid.Empty, string.Empty, string.Empty, false, 0, new List<string>())
    {
    }
}

