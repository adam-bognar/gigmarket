using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Models;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetSellerPublicProfile;

public sealed class GetSellerPublicProfileQueryHandler(
    IApplicationDbContext db,
    IBlobStorageService blobStorageService)
    : IRequestHandler<GetSellerPublicProfileQuery, SellerPublicProfileDto>
{
    public async Task<SellerPublicProfileDto> Handle(
        GetSellerPublicProfileQuery request,
        CancellationToken ct)
    {
        var seller = await db.SellerProfiles
            .AsNoTracking()
            .Include(s => s.Occupation)
            .Include(s => s.Languages).ThenInclude(l => l.Language)
            .Include(s => s.Skills).ThenInclude(sk => sk.Skill)
            .Include(s => s.Educations)
            .Include(s => s.Certifications)
            .FirstOrDefaultAsync(s => s.Id == request.SellerProfileId, ct);

        if (seller is null)
            throw new NotFoundException($"Seller profile '{request.SellerProfileId}' was not found.");

        var rawGigs = await db.Gigs
            .AsNoTracking()
            .Where(g => g.SellerProfileId == request.SellerProfileId && g.Status == GigStatus.Active)
            .Select(g => new
            {
                g.Id,
                g.Title,
                PrimaryPhotoBlobPath = g.Photos.Where(p => p.IsPrimary).Select(p => p.Url).FirstOrDefault() ?? string.Empty,
                StartingPrice = g.Packages.Any() ? g.Packages.Min(p => p.Price) : 0m,
                MinDeliveryDays = g.Packages.Any() ? g.Packages.Min(p => p.DeliveryDays) : 999,
                g.SellerProfileId,
                SellerFirstName = seller.FirstName,
                SellerLastName = seller.LastName,
                SellerAvatarBlobPath = seller.ProfileImageUrl,
                g.CategoryId,
                CategoryName = g.Category.Name,
                SubcategoryName = g.Subcategory.Name,
                AverageRating = g.Reviews.Any() ? Math.Round(g.Reviews.Average(r => r.Rating), 1) : 0.0,
                TotalReviews = g.Reviews.Count(),
                Tags = g.Tags.Select(t => t.Name).ToList(),
                Status = g.Status.ToString()
            })
            .ToListAsync(ct);

        var gigIds = rawGigs.Select(g => g.Id).ToHashSet();

        var allReviews = await db.GigReviews
            .AsNoTracking()
            .Where(r => gigIds.Contains(r.GigId))
            .Select(r => new { r.Rating })
            .ToListAsync(ct);

        var averageRating = allReviews.Count > 0
            ? Math.Round(allReviews.Average(r => r.Rating), 1)
            : 0.0;
        var totalReviews = allReviews.Count;

        var latestReviews = await db.GigReviews
            .AsNoTracking()
            .Include(r => r.Reviewer)
            .Include(r => r.Gig)
            .Where(r => gigIds.Contains(r.GigId))
            .OrderByDescending(r => r.CreatedAtUtc)
            .Take(10)
            .ToListAsync(ct);

        var avatarUrl = await ResolveUrlAsync(seller.ProfileImageUrl, ct);

        var gigCards = await Task.WhenAll(rawGigs.Select(async g => new GigSummaryDto(
            g.Id,
            g.Title,
            await ResolveUrlAsync(g.PrimaryPhotoBlobPath, ct),
            g.StartingPrice,
            g.MinDeliveryDays,
            g.SellerProfileId,
            g.SellerFirstName,
            g.SellerLastName,
            avatarUrl,
            g.CategoryId,
            g.CategoryName,
            g.SubcategoryName,
            g.AverageRating,
            g.TotalReviews,
            g.Tags,
            g.Status
        )));

        var reviewDtos = latestReviews.Select(r => new SellerReviewDto(
            r.Id,
            r.GigId,
            r.Gig.Title,
            r.ReviewerUserId,
            r.Reviewer.CustomUsername,
            r.Rating,
            r.Description,
            r.CreatedAtUtc
        )).ToList();

        return new SellerPublicProfileDto(
            seller.Id,
            seller.UserId,
            seller.FirstName,
            seller.LastName,
            seller.Description,
            avatarUrl,
            seller.PersonalWebsite,
            new SellerOccupationDto(seller.Occupation.Name, seller.Occupation.FromYear, seller.Occupation.ToYear),
            seller.Languages.Select(l => new SellerLanguageDto(l.LanguageId, l.Language.Name)).ToList(),
            seller.Skills.Select(s => s.Skill.Name).ToList(),
            seller.Educations.Select(e => new SellerEducationDto(
                e.Country, e.InstitutionName, e.Degree, e.Major, e.GraduationYear)).ToList(),
            seller.Certifications.Select(c => new SellerCertificationDto(
                c.Name, c.IssuingOrganization, c.Year)).ToList(),
            seller.CreatedAtUtc,
            averageRating,
            totalReviews,
            gigCards.ToList(),
            reviewDtos
        );
    }

    private async Task<string> ResolveUrlAsync(string blobPath, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(blobPath)) return string.Empty;
        if (blobPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            blobPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return blobPath;

        return await blobStorageService.GetDownloadUrlAsync(blobPath, ct);
    }
}