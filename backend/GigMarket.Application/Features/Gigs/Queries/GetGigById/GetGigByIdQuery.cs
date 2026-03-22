using GigMarket.Application.Features.Gigs.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Queries.GetGigById;

public sealed record GetGigByIdQuery(Guid GigId) : IRequest<GigDetailDto>;

