using AutoMapper;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Models;
using GigMarket.Application.Features.Reviews.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Gigs.Queries.GetGigById;

public sealed class GetGigByIdQueryHandler(IApplicationDbContext db, IMapper mapper, IBlobStorageService blobStorageService)
    : IRequestHandler<GetGigByIdQuery, GigDetailDto>
{
    public async Task<GigDetailDto> Handle(GetGigByIdQuery request, CancellationToken cancellationToken)
    {
        var gig = await db.Gigs
            .AsNoTracking()
            .Include(g => g.Category)
            .Include(g => g.Subcategory)
            .Include(g => g.SellerProfile)
            .Include(g => g.Tags)
            .Include(g => g.Packages)
            .Include(g => g.Photos)
            .Include(g => g.Video)
            .Include(g => g.Requirements)
                .ThenInclude(r => r.Choices)
            .Include(g => g.Reviews)
                .ThenInclude(r => r.Reviewer)
            .FirstOrDefaultAsync(g => g.Id == request.GigId, cancellationToken)
            ?? throw new NotFoundException($"Gig with id '{request.GigId}' was not found.");
        
        if (gig.Status == GigStatus.Draft)
            throw new NotFoundException($"Gig with id '{request.GigId}' was not found.");

        var primaryPhotoBlobPath = gig.Photos.FirstOrDefault(p => p.IsPrimary)?.Url ?? string.Empty;
        var additionalPhotoBlobPaths = gig.Photos
            .Where(p => !p.IsPrimary)
            .OrderBy(p => p.SortOrder)
            .Select(p => p.Url)
            .ToList();

        var primaryPhotoUrl = await ResolveUrlAsync(primaryPhotoBlobPath, cancellationToken);

        var additionalPhotoUrls = await Task.WhenAll(additionalPhotoBlobPaths
            .Select(p => ResolveUrlAsync(p, cancellationToken)));

        var videoUrl = await ResolveUrlAsync(gig.Video?.Url, cancellationToken);
        var sellerAvatarUrl = await ResolveUrlAsync(gig.SellerProfile.ProfileImageUrl, cancellationToken);

        var averageRating = gig.Reviews.Any()
            ? Math.Round(gig.Reviews.Average(r => r.Rating), 1)
            : 0.0;

        return new GigDetailDto(
            Id:               gig.Id,
            Title:            gig.Title,
            Description:      gig.Description,
            Status:           gig.Status.ToString(),
            CreatedAtUtc:     gig.CreatedAtUtc,

            CategoryId:       gig.CategoryId,
            CategoryName:     gig.Category.Name,
            SubcategoryId:    gig.SubcategoryId,
            SubcategoryName:  gig.Subcategory.Name,

            SellerProfileId:  gig.SellerProfileId,
            SellerFirstName:  gig.SellerProfile.FirstName,
            SellerLastName:   gig.SellerProfile.LastName,
            SellerAvatarUrl:  sellerAvatarUrl,

            PrimaryPhotoUrl:      primaryPhotoUrl,
            AdditionalPhotoUrls:  additionalPhotoUrls.ToList(),
            VideoUrl:             string.IsNullOrWhiteSpace(videoUrl) ? null : videoUrl,

            Tags: gig.Tags.Select(t => t.Name).ToList(),

            Packages: gig.Packages
                .OrderBy(p => p.Price)
                .Select(p => new GigDetailPackageDto(
                    p.Id,
                    p.Tier.ToString(),
                    p.Name,
                    p.Description,
                    p.DeliveryDays,
                    p.Revisions,
                    p.Price))
                .ToList(),

            Requirements: gig.Requirements
                .OrderBy(r => r.SortOrder)
                .Select(r => new GigDetailRequirementDto(
                    r.Id,
                    r.Type.ToString(),
                    r.Question,
                    r.IsRequired,
                    r.SortOrder,
                    r.Choices.Select(c => c.Value).ToList()))
                .ToList(),

            AverageRating: averageRating,
            TotalReviews:  gig.Reviews.Count,
            Reviews: gig.Reviews
                .OrderByDescending(r => r.CreatedAtUtc)
                .Select(r => mapper.Map<ReviewDto>(r))
                .ToList()
        );
    }

    private async Task<string> ResolveUrlAsync(string? blobPath, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(blobPath)) return string.Empty;
        if (blobPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            blobPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return blobPath;
        }

        return await blobStorageService.GetDownloadUrlAsync(blobPath, ct);
    }
}
