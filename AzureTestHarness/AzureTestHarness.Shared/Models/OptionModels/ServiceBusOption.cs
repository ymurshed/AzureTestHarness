namespace AzureTestHarness.Shared.Models.OptionModels
{
    public class ServiceBusOption
    {
        public string Queue { get; set; }            // Queue Name
        public string Topic { get; set; }            // Topic Name
        public string QueueConnectionString { get; set; } // Service Bus Home -> Shared access policies -> RootManageSharedAccessKey -> Primary Connection String
        public string TopicConnectionString { get; set; } // Service Bus Home -> Shared access policies -> RootManageSharedAccessKey -> Primary Connection String
    }
}
