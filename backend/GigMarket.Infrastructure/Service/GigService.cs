using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.Gigs.Commands.UpdateGig;
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

        var category = await db.GigCategories
                           .AsNoTracking()
                           .FirstOrDefaultAsync(x => x.Id == request.CategoryId, ct)
                       ?? throw new BadRequestException("Invalid category.");

        var subcategory = await db.GigSubcategories
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == request.SubcategoryId, ct)
                          ?? throw new BadRequestException("Invalid subcategory.");

        if (subcategory.CategoryId != category.Id)
            throw new BadRequestException("Subcategory does not belong to the selected category.");

        var gig = new Gig
        {
            Id = Guid.NewGuid(),
            SellerProfileId = sellerProfile.Id,
            CategoryId = category.Id,
            SubcategoryId = subcategory.Id,
            Title = request.Title,
            Description = request.Description,
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
            
            Photos = BuildPhotos(request.PrimaryPhotoUrl, request.AdditionalPhotoUrls),
            
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
            Category: category.Name,
            Subcategory: subcategory.Name,
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

    public async Task DeleteGigAsync(Guid gigId, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var gig = await db.Gigs
            .Include(g => g.SellerProfile)
            .FirstOrDefaultAsync(g => g.Id == gigId, ct)
            ?? throw new NotFoundException($"Gig with id '{gigId}' was not found.");

        if (gig.SellerProfile.UserId != userId)
            throw new UnauthorizedException("You can only delete your own gigs.");

        db.Gigs.Remove(gig);
        await db.SaveChangesAsync(ct);
    }

    public async Task<GigDto> UpdateGigAsync(Guid gigId, UpdateGigRequest request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var gig = await db.Gigs
            .Include(g => g.SellerProfile)
            .Include(g => g.Tags)
            .Include(g => g.Packages)
            .Include(g => g.Requirements)
                .ThenInclude(r => r.Choices)
            .Include(g => g.Photos)
            .Include(g => g.Video)
            .FirstOrDefaultAsync(g => g.Id == gigId, ct)
            ?? throw new NotFoundException($"Gig with id '{gigId}' was not found.");

        if (gig.SellerProfile.UserId != userId)
            throw new UnauthorizedException("You can only edit your own gigs.");

        var category = await db.GigCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId, ct)
            ?? throw new BadRequestException("Invalid category.");

        var subcategory = await db.GigSubcategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.SubcategoryId, ct)
            ?? throw new BadRequestException("Invalid subcategory.");

        if (subcategory.CategoryId != category.Id)
            throw new BadRequestException("Subcategory does not belong to the selected category.");

        gig.Title = request.Title;
        gig.Description = request.Description;
        gig.CategoryId = request.CategoryId;
        gig.SubcategoryId = request.SubcategoryId;

        if (request.Status.HasValue)
            gig.Status = request.Status.Value;

        db.GigTags.RemoveRange(gig.Tags);
        db.GigPackages.RemoveRange(gig.Packages);
        db.GigRequirements.RemoveRange(gig.Requirements);
        db.GigPhotos.RemoveRange(gig.Photos);

        if (gig.Video is not null)
            db.GigVideos.Remove(gig.Video);

        gig.Tags = request.Tags.Select(t => new GigTag { Id = Guid.NewGuid(), Name = t }).ToList();

        gig.Packages = request.Packages.Select(p => new GigPackage
        {
            Id = Guid.NewGuid(),
            Tier = p.Tier,
            Name = p.Name,
            Description = p.Description,
            DeliveryDays = p.DeliveryDays,
            Revisions = p.Revisions,
            Price = p.Price,
        }).ToList();

        gig.Requirements = request.Requirements?.Select(r => new GigRequirement
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
        }).ToList() ?? new List<GigRequirement>();

        gig.Photos = BuildPhotos(request.PrimaryPhotoUrl, request.AdditionalPhotoUrls);

        gig.Video = request.VideoUrl is not null
            ? new GigVideo { Id = Guid.NewGuid(), Url = request.VideoUrl }
            : null;

        await db.SaveChangesAsync(ct);

        return new GigDto(
            Id: gig.Id,
            SellerProfileId: gig.SellerProfileId,
            Title: gig.Title,
            Category: category.Name,
            Subcategory: subcategory.Name,
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
    

    private static List<GigPhoto> BuildPhotos(string primaryPhotoUrl, List<string>? additionalPhotoUrls)
    {
        var photos = new List<GigPhoto>
        {
            new() { Id = Guid.NewGuid(), Url = primaryPhotoUrl, IsPrimary = true, SortOrder = 0 }
        };

        if (additionalPhotoUrls is not null)
        {
            photos.AddRange(additionalPhotoUrls.Select((url, i) => new GigPhoto
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