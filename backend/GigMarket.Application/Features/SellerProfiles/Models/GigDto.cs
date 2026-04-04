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
)
{
    public GigDto() : this(
        Guid.Empty,
        Guid.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        DateTime.UtcNow,
        new List<string>(),
        new List<GigPackageDto>(),
        new GigPhotosDto())
    {
    }
}

public sealed record GigPackageDto(
    Guid Id,
    string Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price
)
{
    public GigPackageDto() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0m)
    {
    }
}

public sealed record GigPhotosDto(
    string PrimaryPhotoUrl,
    List<string> AdditionalPhotoUrls
)
{
    public GigPhotosDto() : this(string.Empty, new List<string>())
    {
    }
}
