using MediatR;

namespace GigMarket.Application.Features.Files.Commands.DeleteFile;

public sealed record DeleteFileCommand(string BlobPath) : IRequest;

