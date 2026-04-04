using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.CreateGig;

public sealed record CreateGigCommand(CreateGigRequest GigRequest) : IRequest<GigDto>;

public sealed record CreateGigRequest(
    Guid GigId,
    string Title,
    Guid CategoryId,
    Guid SubcategoryId,
    List<string> Tags,
    string Description,

    List<GigPackageRequest> Packages,

    List<GigRequirementRequest>? Requirements,

    string PrimaryPhotoUrl,
    List<string>? AdditionalPhotoUrls,
    string? VideoUrl
)
{
    public CreateGigRequest() : this(
        Guid.Empty,
        string.Empty,
        Guid.Empty,
        Guid.Empty,
        new List<string>(),
        string.Empty,
        new List<GigPackageRequest>(),
        new List<GigRequirementRequest>(),
        string.Empty,
        new List<string>(),
        null)
    {
    }
}

public sealed record GigPackageRequest(
    PackageTier Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price)
{
    public GigPackageRequest() : this(default, string.Empty, string.Empty, 0, 0, 0m)
    {
    }
}
    
public sealed record GigRequirementRequest(
    RequirementType Type,
    string Question,
    bool IsRequired,
    int SortOrder,
    List<string>? Choices)
{
    public GigRequirementRequest() : this(default, string.Empty, false, 0, new List<string>())
    {
    }
}
