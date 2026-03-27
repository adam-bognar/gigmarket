using GigMarket.Application.Features.Gigs.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Queries.GetMyGigs;

public sealed record GetMyGigsQuery : IRequest<List<GigSummaryDto>>;

