using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.UpdateGig;

public sealed record UpdateGigCommand(Guid GigId, UpdateGigRequest GigRequest) : IRequest<GigDto>;

public sealed record UpdateGigRequest(
    string Title,
    Guid CategoryId,
    Guid SubcategoryId,
    List<string> Tags,
    string Description,
    List<UpdateGigPackageRequest> Packages,
    List<UpdateGigRequirementRequest>? Requirements,
    string PrimaryPhotoUrl,
    List<string>? AdditionalPhotoUrls,
    string? VideoUrl,
    GigStatus? Status
)
{
    public UpdateGigRequest() : this(
        string.Empty,
        Guid.Empty,
        Guid.Empty,
        new List<string>(),
        string.Empty,
        new List<UpdateGigPackageRequest>(),
        new List<UpdateGigRequirementRequest>(),
        string.Empty,
        new List<string>(),
        null,
        null)
    {
    }
}

public sealed record UpdateGigPackageRequest(
    PackageTier Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price)
{
    public UpdateGigPackageRequest() : this(default, string.Empty, string.Empty, 0, 0, 0m)
    {
    }
}

public sealed record UpdateGigRequirementRequest(
    RequirementType Type,
    string Question,
    bool IsRequired,
    int SortOrder,
    List<string>? Choices)
{
    public UpdateGigRequirementRequest() : this(default, string.Empty, false, 0, new List<string>())
    {
    }
}

