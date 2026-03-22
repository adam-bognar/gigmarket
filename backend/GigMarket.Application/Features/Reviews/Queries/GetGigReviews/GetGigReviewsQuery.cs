using GigMarket.Application.Features.Reviews.Models;
using MediatR;

namespace GigMarket.Application.Features.Reviews.Queries.GetGigReviews;

public sealed record GetGigReviewsQuery(Guid GigId) : IRequest<GetGigReviewsResult>;

public sealed record GetGigReviewsResult(
    Guid GigId,
    double AverageRating,
    int TotalReviews,
    List<ReviewDto> Reviews
);

