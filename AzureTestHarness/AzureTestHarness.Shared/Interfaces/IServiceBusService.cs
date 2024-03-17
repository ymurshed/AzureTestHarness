using System.Threading.Tasks;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IServiceBusService
    {
        Task SendQueueMessageAsync<T>(T serviceBusMessage);
        Task<string> ReceiveQueueMessageAsync();
        Task ProcessQueueMessageAsync();

        Task<string> ReceiveTopicMessageAsync(string subscriptionName);
        Task ProcessTopicMessageAsync(string subscriptionName);
        
        Task GetSubscriptionsAsync();
    }
}
