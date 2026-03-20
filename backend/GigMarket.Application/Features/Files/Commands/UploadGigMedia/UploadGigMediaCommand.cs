using GigMarket.Application.Features.Files.Models;
using MediatR;

namespace GigMarket.Application.Features.Files.Commands.UploadGigMedia;

public sealed record UploadGigMediaCommand(
    Guid GigId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileLength) : IRequest<UploadedFileDto>;

