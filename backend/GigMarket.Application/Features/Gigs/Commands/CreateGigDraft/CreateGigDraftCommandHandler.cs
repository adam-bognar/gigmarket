using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.CreateGigDraft;

public class CreateGigDraftCommandHandler(IGigService gigService) : IRequestHandler<CreateGigDraftCommand, GigDto>
{
    public Task<GigDto> Handle(CreateGigDraftCommand request, CancellationToken ct)
        => gigService.CreateGigDraftAsync(ct);
}