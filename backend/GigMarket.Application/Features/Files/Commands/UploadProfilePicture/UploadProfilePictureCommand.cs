using GigMarket.Application.Features.Files.Models;
using MediatR;

namespace GigMarket.Application.Features.Files.Commands.UploadProfilePicture;

public sealed record UploadProfilePictureCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileLength) : IRequest<UploadedFileDto>;

