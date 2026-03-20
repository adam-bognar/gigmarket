using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.CreateGig;

public sealed record CreateGigCommand(CreateGigRequest GigRequest) : IRequest<GigDto>;

public sealed record CreateGigRequest(
    string Title,
    string Category,
    string Subcategory,
    List<string> Tags,
    string Description,

    List<GigPackageRequest> Packages,

    List<GigRequirementRequest>? Requirements,

    string PrimaryPhotoUrl,
    List<string>? AdditionalPhotoUrls,
    string? VideoUrl
);

public sealed record GigPackageRequest(
    PackageTier Tier,
    string Name,
    string Description,
    int DeliveryDays,
    int Revisions,
    decimal Price);
    
public sealed record GigRequirementRequest(
    RequirementType Type,
    string Question,
    bool IsRequired,
    int SortOrder,
    List<string>? Choices);