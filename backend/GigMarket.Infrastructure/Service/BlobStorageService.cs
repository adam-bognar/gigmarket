using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace GigMarket.Infrastructure.Service;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private BlobContainerClient _containerClient;
    private readonly StorageSharedKeyCredential _sharedKeyCredential;

    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerClient = blobServiceClient.GetBlobContainerClient("uploads");

        var accountName = configuration["AzureStorage:AccountName"];
        var accountKey = configuration["AzureStorage:AccountKey"];
        _sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder,
        CancellationToken cancellationToken)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueName = $"{folder}/{Guid.NewGuid()}{ext}";

        var blobClient = _containerClient.GetBlobClient(uniqueName);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, uploadOptions, cancellationToken);
        return uniqueName;
    }

    public async Task<List<string>> ListBlobsAsync(string folder, CancellationToken cancellationToken)
    {
        var list = new List<string>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, folder, cancellationToken: cancellationToken))
        {
            list.Add(blobItem.Name);
        }

        return list;
    }

    public async Task DeleteFileAsync(string blobPath, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobPath);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobPath);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return Task.FromResult(sasUri.AbsoluteUri);
    }
}