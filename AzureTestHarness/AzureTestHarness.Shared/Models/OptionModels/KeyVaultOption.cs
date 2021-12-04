namespace AzureTestHarness.Shared.Models.OptionModels
{
    public class KeyVaultOption
    {
        public string Uri { get; set; }             // Key Vault -> Overview -> Vault URI
        public string TenantId { get; set; }        // App Registration -> Overview -> Directory (tenant) ID
        public string ClientId { get; set; }        // App Registration -> Overview -> Application (client) ID
        public string ClientSecret { get; set; }    // App Registration -> Overview -> Client credentials -> Value
    }
}
