using GigMarket.Application.Common.Interfaces;

namespace GigMarket.Infrastructure.Service;

public class BlobUrlResolverService(IBlobStorageService blobStorageService) : IBlobUrlResolverService
{
    public async Task<string> ResolveUrlAsync(string blobPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
            return string.Empty;

        if (blobPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            blobPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return blobPath;

        return await blobStorageService.GetDownloadUrlAsync(blobPath, cancellationToken);
    }
}