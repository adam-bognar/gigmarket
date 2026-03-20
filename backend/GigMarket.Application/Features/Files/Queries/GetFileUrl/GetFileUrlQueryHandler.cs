using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Files.Models;
using MediatR;

namespace GigMarket.Application.Features.Files.Queries.GetFileUrl;

public sealed class GetFileUrlQueryHandler(IBlobStorageService blobStorageService)
    : IRequestHandler<GetFileUrlQuery, FileDownloadUrlDto>
{
    public async Task<FileDownloadUrlDto> Handle(GetFileUrlQuery request, CancellationToken cancellationToken)
    {
        var url = await blobStorageService.GetDownloadUrlAsync(request.BlobPath, cancellationToken);
        return new FileDownloadUrlDto(url);
    }
}

