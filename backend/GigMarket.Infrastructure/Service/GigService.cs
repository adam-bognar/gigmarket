using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Infrastructure.Service;

public class GigService(ICurrentUserService currentUser, IApplicationDbContext db) : IGigService
{
    public async Task<GigDto> CreateGigAsync(CreateGigRequest request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var sellerProfile = await db.SellerProfiles
                                .FirstOrDefaultAsync(x => x.UserId == userId, ct)
                            ?? throw new BadRequestException("You must complete your seller profile first.");

        var gig = new Gig
        {
            Id = Guid.NewGuid(),
            SellerProfileId = sellerProfile.Id,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Subcategory = request.Subcategory,
            Tags = request.Tags.Select(t => new GigTag { Name = t }).ToList(),
            Packages = request.Packages.Select(p => new GigPackage
            {
                Id = Guid.NewGuid(),
                Tier = p.Tier,
                Name = p.Name,
                Description = p.Description,
                DeliveryDays = p.DeliveryDays,
                Revisions = p.Revisions,
                Price = p.Price,
            }).ToList(),
            
            Requirements = request.Requirements?.Select(r => new GigRequirement
            {
                Id = Guid.NewGuid(),
                Type = r.Type,
                Question = r.Question,
                IsRequired = r.IsRequired,
                SortOrder = r.SortOrder,
                Choices = r.Choices?.Select(c => new GigRequirementChoice
                {
                    Id = Guid.NewGuid(),
                    Value = c
                }).ToList() ?? new List<GigRequirementChoice>()
            }).ToList() ?? new List<GigRequirement>(),
            
            Photos = BuildPhotos(request),
            
            Video = request.VideoUrl is not null
                ? new GigVideo { Id = Guid.NewGuid(), Url = request.VideoUrl }
                : null
        };
        
        db.Gigs.Add(gig);
        await db.SaveChangesAsync(ct);

        return new GigDto(
            Id: gig.Id,
            SellerProfileId: gig.SellerProfileId,
            Title: gig.Title,
            Category: gig.Category,
            Subcategory: gig.Subcategory,
            Status: gig.Status.ToString(),
            CreatedAtUtc: gig.CreatedAtUtc,
            Tags: gig.Tags.Select(t => t.Name).ToList(),
            Packages: gig.Packages.Select(p => new GigPackageDto(
                p.Id, p.Tier.ToString(), p.Name, p.Description,
                p.DeliveryDays, p.Revisions, p.Price)).ToList(),
            Photos: new GigPhotosDto(
                PrimaryPhotoUrl: gig.Photos.First(p => p.IsPrimary).Url,
                AdditionalPhotoUrls: gig.Photos.Where(p => !p.IsPrimary)
                    .OrderBy(p => p.SortOrder)
                    .Select(p => p.Url).ToList())
        );
    }
    
    private static List<GigPhoto> BuildPhotos(CreateGigRequest req)
    {
        var photos = new List<GigPhoto>
        {
            new() { Id = Guid.NewGuid(), Url = req.PrimaryPhotoUrl, IsPrimary = true, SortOrder = 0 }
        };

        if (req.AdditionalPhotoUrls is not null)
        {
            photos.AddRange(req.AdditionalPhotoUrls.Select((url, i) => new GigPhoto
            {
                Id = Guid.NewGuid(),
                Url = url,
                IsPrimary = false,
                SortOrder = i + 1
            }));
        }

        return photos;
    }
}