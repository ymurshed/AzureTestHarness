using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.Extensions.Options;

namespace AzureTestHarness.Services.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly IOptions<KeyVaultOption> _keyVaultOption;
        private readonly SecretClient _client;

        public KeyVaultService(IOptions<KeyVaultOption> keyVaultOption)
        {
            _keyVaultOption = keyVaultOption;
            _client = GetClient();
        }

        public async Task<Azure.Response<KeyVaultSecret>> GetSecretAsync(string name)
        {
            try
            {
                var response = await _client.GetSecretAsync(name);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KeyVault getting secret failed. Error: {ex}");
                return null;
            }
        }

        public async Task<Azure.Response<KeyVaultSecret>> SetSecretAsync(string name, string value)
        {
            try
            {
                var secret = new KeyVaultSecret(name, value);
                var response = await _client.SetSecretAsync(secret);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KeyVault setting secret failed. Error: {ex}");
                return null;
            }
        }

        #region private methods
        private SecretClient GetClient()
        {
            var client = new SecretClient(new Uri(_keyVaultOption.Value.Uri),
                                          new ClientSecretCredential(_keyVaultOption.Value.TenantId, _keyVaultOption.Value.ClientId, _keyVaultOption.Value.ClientSecret));
            return client;
        }
        #endregion
    }
}
