using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace HybridMessenger.Infrastructure.Services
{
    public class KeyVaultService
    {
        private SecretClient _secretClient;
        private IConfiguration _configuration;

        public KeyVaultService(IConfiguration configuration)
        {
            _configuration = configuration;
            var kvUri = _configuration["KeyVaultUrl"];
            _secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        }

        public string GetDbConnectionString()
        {
            KeyVaultSecret secret = _secretClient.GetSecret("RemoteDbConnection");
            return secret.Value;
        }

        public string GetJwtKey()
        {
            KeyVaultSecret secret = _secretClient.GetSecret("JwtKey");
            return secret.Value;
        }

        public string GetBlobConnection()
        {
            KeyVaultSecret secret = _secretClient.GetSecret("BlobStorageConnection");
            return secret.Value;
        }

        public string GetBlobContainerName()
        {
            KeyVaultSecret secret = _secretClient.GetSecret("BlobContainerName");
            return secret.Value;
        }
    }
}