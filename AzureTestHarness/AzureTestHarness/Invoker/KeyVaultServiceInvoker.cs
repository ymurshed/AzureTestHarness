using System;
using System.Threading.Tasks;
using AzureTestHarness.Shared.Interfaces;

namespace AzureTestHarness.Invoker
{
    public class KeyVaultServiceInvoker : IInvoker
    {
        private readonly IKeyVaultService _keyVaultService;

        public KeyVaultServiceInvoker(IKeyVaultService keyVaultService)
        {
            Console.WriteLine("\nCalling Azure Key Vault Service --->>> ");
            _keyVaultService = keyVaultService;
        }

        public async Task Invoke()
        {
            const string name = "UserName";
            var response = await _keyVaultService.GetSecretAsync(name);
            Console.WriteLine(response != null ? $"Secret found. Name = {response.Value.Name}, Value = {response.Value.Value}" : $"Secret '{name}' not found!");

            var (key, value) = Tuple.Create("Designation", "Principal Software Engineer");
            response = await _keyVaultService.SetSecretAsync(key, value);
            Console.WriteLine(response != null ? $"Secret set. Name = {response.Value.Name}, Value = {response.Value.Value}" : $"Secret '{name}' not set!");
        }
    }
}
