using GigMarket.Application.Common.Interfaces;
using MediatR;

namespace GigMarket.Application.Features.Files.Commands.DeleteFile;

public sealed class DeleteFileCommandHandler(IBlobStorageService blobStorageService)
    : IRequestHandler<DeleteFileCommand>
{
    public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        await blobStorageService.DeleteFileAsync(request.BlobPath, cancellationToken);
    }
}

