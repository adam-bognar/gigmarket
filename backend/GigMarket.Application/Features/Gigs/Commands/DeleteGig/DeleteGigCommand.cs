using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.DeleteGig;

public sealed record DeleteGigCommand(Guid GigId) : IRequest<Unit>;

