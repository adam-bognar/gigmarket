using AutoMapper;
using AutoMapper.QueryableExtensions;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Reviews.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Reviews.Queries.GetGigReviews;

public sealed class GetGigReviewsQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetGigReviewsQuery, GetGigReviewsResult>
{
    public async Task<GetGigReviewsResult> Handle(GetGigReviewsQuery request, CancellationToken cancellationToken)
    {
        var gigExists = await db.Gigs
            .AsNoTracking()
            .AnyAsync(g => g.Id == request.GigId, cancellationToken);

        if (!gigExists)
            throw new NotFoundException($"Gig with id '{request.GigId}' was not found.");

        var reviews = await db.GigReviews
            .AsNoTracking()
            .Include(r => r.Reviewer)
            .Where(r => r.GigId == request.GigId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ProjectTo<ReviewDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var averageRating = reviews.Count > 0
            ? Math.Round(reviews.Average(r => r.Rating), 1)
            : 0.0;

        return new GetGigReviewsResult(
            request.GigId,
            averageRating,
            reviews.Count,
            reviews);
    }
}

