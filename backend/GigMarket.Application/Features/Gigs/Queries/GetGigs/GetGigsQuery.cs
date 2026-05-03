using GigMarket.Application.Features.Gigs.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Queries.GetGigs;

public sealed record GetGigsQuery(
    string? Search = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? DeliveryTime = null,
    double? MinRating = null,
    string? SortBy = null
) : IRequest<List<GigSummaryDto>>;