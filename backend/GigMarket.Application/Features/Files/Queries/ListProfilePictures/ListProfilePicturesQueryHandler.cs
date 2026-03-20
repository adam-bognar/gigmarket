using GigMarket.Application.Common.Interfaces;
using MediatR;

namespace GigMarket.Application.Features.Files.Queries.ListProfilePictures;

public sealed class ListProfilePicturesQueryHandler(IBlobStorageService blobStorageService)
    : IRequestHandler<ListProfilePicturesQuery, List<string>>
{
    public async Task<List<string>> Handle(ListProfilePicturesQuery request, CancellationToken cancellationToken)
    {
        return await blobStorageService.ListBlobsAsync($"profiles/{request.UserId}", cancellationToken);
    }
}

