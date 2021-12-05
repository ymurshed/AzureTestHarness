namespace AzureTestHarness.Shared.Models.OptionModels
{
    public class ServiceBusOption
    {
        public string Queue { get; set; }            // Queue Name
        public string ConnectionString { get; set; } // Service Bus Home -> Shared access policies -> RootManageSharedAccessKey -> Primary Connection String
    }
}
