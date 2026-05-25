using System.Diagnostics;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Gigs.Queries.GetGigs;

public sealed class GetGigsQueryHandler(IApplicationDbContext db, IBlobStorageService blobStorageService)
    : IRequestHandler<GetGigsQuery, List<GigSummaryDto>>
{
    public async Task<List<GigSummaryDto>> Handle(GetGigsQuery request, CancellationToken cancellationToken)
    {
        int? maxDeliveryDays = request.DeliveryTime switch
        {
            "24h"   => 1,
            "3days" => 3,
            "7days" => 7,
            _       => null,
        };

        var query = db.Gigs
            .AsNoTracking()
            .Where(g => string.IsNullOrEmpty(request.Search) || g.Title.Contains(request.Search))
            .Where(g => request.CategoryId == null || g.CategoryId == request.CategoryId)
            .Where(g => request.MinPrice == null || (g.Packages.Any() && g.Packages.Min(p => p.Price) >= request.MinPrice))
            .Where(g => request.MaxPrice == null || (g.Packages.Any() && g.Packages.Min(p => p.Price) <= request.MaxPrice))
            .Where(g => maxDeliveryDays == null || (g.Packages.Any() && g.Packages.Min(p => p.DeliveryDays) <= maxDeliveryDays))
            .Where(g => request.MinRating == null || request.MinRating == 0 || (g.Reviews.Any() && g.Reviews.Average(r => r.Rating) >= request.MinRating))
            .Where(g => g.Status == GigStatus.Active);

        var sortedQuery = request.SortBy switch
        {
            "price_asc" => query.OrderBy(g => g.Packages.Select(p => (decimal?)p.Price).Min() ?? 0m),
            "price_desc" => query.OrderByDescending(g => g.Packages.Select(p => (decimal?)p.Price).Min() ?? 0m),
            "rating_desc" => query.OrderByDescending(g => g.Reviews.Select(r => (double?)r.Rating).Average() ?? 0.0),
            "reviews_desc" => query.OrderByDescending(g => g.Reviews.Count()),
            _              => query.OrderByDescending(g => g.Reviews.Count()),
        };

        var sw = Stopwatch.StartNew();

        var rawGigs = await sortedQuery
            .Select(g => new
            {
                g.Id,
                g.Title,
                PrimaryPhotoBlobPath = g.Photos.Where(p => p.IsPrimary).Select(p => p.Url).FirstOrDefault() ?? string.Empty,
                StartingPrice = g.Packages
                    .Select(p => (decimal?)p.Price)
                    .Min() ?? 0m,
                MinDeliveryDays = g.Packages
                    .Select(p => (int?)p.DeliveryDays)
                    .Min() ?? 999,
                g.SellerProfileId,
                SellerFirstName = g.SellerProfile.FirstName,
                SellerLastName = g.SellerProfile.LastName,
                SellerAvatarBlobPath = g.SellerProfile.ProfileImageUrl,
                g.CategoryId,
                CategoryName = g.Category.Name,
                SubcategoryName = g.Subcategory.Name,
                AverageRating = g.Reviews
                    .Select(r => (double?)r.Rating)
                    .Average() ?? 0.0,
                TotalReviews = g.Reviews.Count(),
                Tags = g.Tags.Select(t => t.Name).ToList(),
                Status = g.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        Console.WriteLine($"GetGigs DB query took: {sw.ElapsedMilliseconds} ms");

        sw.Restart();

        var cards = await Task.WhenAll(rawGigs.Select(async g => new GigSummaryDto(
            g.Id,
            g.Title,
            await ResolveUrlAsync(g.PrimaryPhotoBlobPath, cancellationToken),
            g.StartingPrice,
            g.MinDeliveryDays,
            g.SellerProfileId,
            g.SellerFirstName,
            g.SellerLastName,
            await ResolveUrlAsync(g.SellerAvatarBlobPath, cancellationToken),
            g.CategoryId,
            g.CategoryName,
            g.SubcategoryName,
            g.AverageRating,
            g.TotalReviews,
            g.Tags,
            g.Status
        )));

        Console.WriteLine($"GetGigs URL resolving took: {sw.ElapsedMilliseconds} ms");

        return cards.ToList();
    }

    private async Task<string> ResolveUrlAsync(string blobPath, CancellationToken ct)
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