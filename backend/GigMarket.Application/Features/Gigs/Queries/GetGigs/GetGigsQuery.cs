using GigMarket.Application.Features.Gigs.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Queries.GetGigs;

public sealed record GetGigsQuery : IRequest<List<GigSummaryDto>>;

