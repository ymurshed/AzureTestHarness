using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.Extensions.Options;

namespace AzureTestHarness.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly IOptions<KeyVaultOption> _keyVaultOption;

        public KeyVaultService(IOptions<KeyVaultOption> keyVaultOption)
        {
            _keyVaultOption = keyVaultOption;
        }

        public async Task<string> GetSecretAsync()
        {
            try
            {
                var client = new SecretClient(
                                                new Uri(_keyVaultOption.Value.Uri),
                                                new ClientSecretCredential(_keyVaultOption.Value.TenantId, _keyVaultOption.Value.ClientId, _keyVaultOption.Value.ClientSecret)
                                             );

                var secret = await client.GetSecretAsync("UserName");
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting secret: {ex}");
                return null;
            }
        }
    }
}
