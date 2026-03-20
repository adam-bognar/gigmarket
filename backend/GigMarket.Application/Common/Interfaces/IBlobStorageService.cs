using GigMarket.Domain.Entities;

namespace GigMarket.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName,
        string contentType, string folder, CancellationToken cancellationToken);
    Task<List<string>> ListBlobsAsync(string folder, CancellationToken cancellationToken);
    Task DeleteFileAsync(string blobPath, CancellationToken cancellationToken);
    Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken cancellationToken);
}