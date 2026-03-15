using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.CreateGig;

public class CreateGigCommandHandler(IGigService gigService) : IRequestHandler<CreateGigCommand, GigDto>
{
    public Task<GigDto> Handle(CreateGigCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}