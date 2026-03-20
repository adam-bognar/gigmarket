using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Files.Models;
using MediatR;

namespace GigMarket.Application.Features.Files.Commands.UploadGigMedia;

public sealed class UploadGigMediaCommandHandler(IBlobStorageService blobStorageService)
    : IRequestHandler<UploadGigMediaCommand, UploadedFileDto>
{
    public async Task<UploadedFileDto> Handle(UploadGigMediaCommand request, CancellationToken cancellationToken)
    {
        var blobPath = await blobStorageService.UploadFileAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            $"gigs/{request.GigId}",
            cancellationToken);

        return new UploadedFileDto(blobPath);
    }
}

