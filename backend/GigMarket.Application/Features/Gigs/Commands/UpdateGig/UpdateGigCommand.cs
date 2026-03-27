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
);

public sealed record UpdateGigPackageRequest(
    PackageTier Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price);

public sealed record UpdateGigRequirementRequest(
    RequirementType Type,
    string Question,
    bool IsRequired,
    int SortOrder,
    List<string>? Choices);

