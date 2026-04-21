using MediatR;

namespace GigMarket.Application.Features.Files.Commands.UploadOrderDeliveryFile;

public sealed record UploadOrderDeliveryFileCommand(
    Guid OrderId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long Length) : IRequest<UploadOrderDeliveryFileResult>;

public sealed record UploadOrderDeliveryFileResult(string Url);