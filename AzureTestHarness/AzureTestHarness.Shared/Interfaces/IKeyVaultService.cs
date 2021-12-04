using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretAsync();
        Task<KeyVaultSecret> SetSecretAsync();
    }
}
