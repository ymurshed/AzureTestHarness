using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IKeyVaultService
    {
        Task<Azure.Response<KeyVaultSecret>> GetSecretAsync(string name);
        Task<Azure.Response<KeyVaultSecret>> SetSecretAsync(string name, string value);
    }
}
