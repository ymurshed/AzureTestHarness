using System.Threading.Tasks;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretAsync();
    }
}
