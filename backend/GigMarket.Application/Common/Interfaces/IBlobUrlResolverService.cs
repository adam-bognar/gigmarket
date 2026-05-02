namespace GigMarket.Application.Common.Interfaces;

public interface IBlobUrlResolverService
{
    Task<string> ResolveUrlAsync(string blobPath, CancellationToken cancellationToken);
}