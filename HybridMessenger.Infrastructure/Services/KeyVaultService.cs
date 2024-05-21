using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace HybridMessenger.Infrastructure.Services
{
    public class KeyVaultService
    {
        private SecretClient _secretClient;

        public KeyVaultService()
        {
            var kvUri = "https://hybridmessengerkeyvault2.vault.azure.net/";
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
    }
}