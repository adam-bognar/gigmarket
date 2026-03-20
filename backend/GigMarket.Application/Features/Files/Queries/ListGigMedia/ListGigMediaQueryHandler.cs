using GigMarket.Application.Common.Interfaces;
using MediatR;

namespace GigMarket.Application.Features.Files.Queries.ListGigMedia;

public sealed class ListGigMediaQueryHandler(IBlobStorageService blobStorageService)
    : IRequestHandler<ListGigMediaQuery, List<string>>
{
    public async Task<List<string>> Handle(ListGigMediaQuery request, CancellationToken cancellationToken)
    {
        return await blobStorageService.ListBlobsAsync($"gigs/{request.GigId}", cancellationToken);
    }
}

