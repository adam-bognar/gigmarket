using GigMarket.Application.Common.Interfaces;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.DeleteGig;

public sealed class DeleteGigCommandHandler(IGigService gigService) : IRequestHandler<DeleteGigCommand, Unit>
{
    public async Task<Unit> Handle(DeleteGigCommand request, CancellationToken cancellationToken)
    {
        await gigService.DeleteGigAsync(request.GigId, cancellationToken);
        return Unit.Value;
    }
}

