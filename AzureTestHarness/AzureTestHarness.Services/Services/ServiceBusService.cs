using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.Extensions.Options;

namespace AzureTestHarness.Services.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly IOptions<ServiceBusOption> _serviceBusOption;
        private readonly ServiceBusClient _client;

        public ServiceBusService(IOptions<ServiceBusOption> serviceBusOption)
        {
            _serviceBusOption = serviceBusOption;
            _client = GetClient();
        }

        ~ServiceBusService()
        {
            _client.DisposeAsync();
        }


        public async Task SendMessageAsync<T>(T serviceBusMessage)
        {
            var sender = _client.CreateSender(_serviceBusOption.Value.Queue);

            try
            {
                var messageBody = JsonSerializer.Serialize(serviceBusMessage);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                
                await sender.SendMessageAsync(message);
                await sender.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus message send failed. Error: {ex}");
            }
            finally
            {
                await sender.CloseAsync();
            }
        }

        // Equivalent to Peek event, perform read only from queue
        public async Task<string> ReceiveMessageAsync()
        {
            var receiver = _client.CreateReceiver(_serviceBusOption.Value.Queue);

            try
            {
                var message = await receiver.ReceiveMessageAsync();
                var jsonString = Encoding.UTF8.GetString(message.Body);
                await receiver.CloseAsync();
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus message receive failed. Error: {ex}");
                return null;
            }
            finally
            {
                await receiver.CloseAsync();
            }
        }

        // Equivalent to Receive event, perform read & delete from queue 
        public async Task ProcessMessageAsync()
        {
            var processor = _client.CreateProcessor(_serviceBusOption.Value.Queue, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();
                Thread.Sleep(30 * 1000);
                await processor.StopProcessingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus message receive failed. Error: {ex}");
            }
            finally
            {
                await processor.CloseAsync();
                await _client.DisposeAsync();
            }
        }

        #region private methods
        private ServiceBusClient GetClient()
        {
            var client = new ServiceBusClient(_serviceBusOption.Value.ConnectionString);
            return client;
        }

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var jsonString = Encoding.UTF8.GetString(args.Message.Body);
            await args.CompleteMessageAsync(args.Message);
            Console.WriteLine($"Message received from service bus: {jsonString}");
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Service bus message receive failed. Error: {args.Exception}");
            return Task.CompletedTask;
        }
        #endregion
    }
}
