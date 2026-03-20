using GigMarket.Application.Features.Files.Models;
using MediatR;

namespace GigMarket.Application.Features.Files.Queries.GetFileUrl;

public sealed record GetFileUrlQuery(string BlobPath) : IRequest<FileDownloadUrlDto>;

