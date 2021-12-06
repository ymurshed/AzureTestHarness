using System.Threading.Tasks;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IServiceBusService
    {
        Task SendMessageAsync<T>(T serviceBusMessage);
        Task<string> ReceiveMessageAsync();
        Task ProcessMessageAsync();
    }
}
