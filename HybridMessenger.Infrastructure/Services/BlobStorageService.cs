using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HybridMessenger.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace HybridMessenger.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly KeyVaultService _vaultService;

        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _vaultService = new KeyVaultService(configuration);

            var connectionString = _vaultService.GetBlobConnection();
            _containerName = _vaultService.GetBlobContainerName();

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "image/jpeg" });
            return blobClient.Uri.ToString();
        }
    }
}
