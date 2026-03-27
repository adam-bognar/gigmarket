using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.UpdateGig;

public sealed class UpdateGigCommandHandler(IGigService gigService) : IRequestHandler<UpdateGigCommand, GigDto>
{
    public Task<GigDto> Handle(UpdateGigCommand request, CancellationToken cancellationToken)
        => gigService.UpdateGigAsync(request.GigId, request.GigRequest, cancellationToken);
}

