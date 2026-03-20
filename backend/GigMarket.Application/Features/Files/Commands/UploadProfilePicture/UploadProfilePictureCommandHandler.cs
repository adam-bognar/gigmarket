using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Features.Files.Models;
using MediatR;

namespace GigMarket.Application.Features.Files.Commands.UploadProfilePicture;

public sealed class UploadProfilePictureCommandHandler(
    IBlobStorageService blobStorageService,
    ICurrentUserService currentUser)
    : IRequestHandler<UploadProfilePictureCommand, UploadedFileDto>
{
    public async Task<UploadedFileDto> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
        {
            throw new UnauthorizedException("Not authenticated.");
        }

        var userId = currentUser.UserId.Value;
        var profileFolder = $"profiles/{userId}";

        var existingBlobs = await blobStorageService.ListBlobsAsync(profileFolder, cancellationToken);
        foreach (var existingBlob in existingBlobs)
        {
            await blobStorageService.DeleteFileAsync(existingBlob, cancellationToken);
        }

        var blobPath = await blobStorageService.UploadFileAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            profileFolder,
            cancellationToken);

        return new UploadedFileDto(blobPath);
    }
}

