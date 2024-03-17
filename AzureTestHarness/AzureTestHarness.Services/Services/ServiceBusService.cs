using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.Extensions.Azure;
using System.Collections.Generic;

namespace AzureTestHarness.Services.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly IOptions<ServiceBusOption> _serviceBusOption;
        private readonly ServiceBusClient _queueClient;
        private readonly ServiceBusClient _topicClient;
        private readonly ServiceBusAdministrationClient _topicAdminClient;
        
        public ServiceBusService(IOptions<ServiceBusOption> serviceBusOption)
        {
            _serviceBusOption = serviceBusOption;
            _queueClient = GetQueueClient();
            _topicClient = GetTopicClient();

            _topicAdminClient = new ServiceBusAdministrationClient(serviceBusOption.Value.TopicConnectionString);
        }

        ~ServiceBusService()
        {
            _queueClient.DisposeAsync();
            _topicClient.DisposeAsync();
        }

        #region Queue
        public async Task SendQueueMessageAsync<T>(T serviceBusMessage)
        {
            var sender = _queueClient.CreateSender(_serviceBusOption.Value.Queue);

            try
            {
                var messageBody = JsonSerializer.Serialize(serviceBusMessage);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                
                await sender.SendMessageAsync(message);
                await sender.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus queue message send failed. Error: {ex}");
            }
            finally
            {
                await sender.CloseAsync();
            }
        }

        // Equivalent to Peek event, perform read only from queue
        public async Task<string> ReceiveQueueMessageAsync()
        {
            var receiver = _queueClient.CreateReceiver(_serviceBusOption.Value.Queue);

            try
            {
                var message = await receiver.ReceiveMessageAsync();
                var jsonString = Encoding.UTF8.GetString(message.Body);
                await receiver.CloseAsync();
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus queue message receive failed. Error: {ex}");
                return null;
            }
            finally
            {
                await receiver.CloseAsync();
            }
        }

        // Equivalent to Receive event, perform read & delete from queue 
        public async Task ProcessQueueMessageAsync()
        {
            var processor = _queueClient.CreateProcessor(_serviceBusOption.Value.Queue, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += QueueMessageHandler;
                processor.ProcessErrorAsync += QueueErrorHandler;

                await processor.StartProcessingAsync();
                Thread.Sleep(30 * 1000);
                await processor.StopProcessingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus queue message receive failed. Error: {ex}");
            }
            finally
            {
                await processor.CloseAsync();
                await _queueClient.DisposeAsync();
            }
        }
        #endregion

        #region Topic
        // Equivalent to Peek event, perform read only from topic
        public async Task<string> ReceiveTopicMessageAsync(string subscriptionName)
        {
            var receiver = _topicClient.CreateReceiver(_serviceBusOption.Value.Topic, subscriptionName);
            
            try
            {
                var message = await receiver.ReceiveMessageAsync();
                var jsonString = Encoding.UTF8.GetString(message.Body);
                await receiver.CloseAsync();
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus topic message receive failed. Error: {ex}");
                return null;
            }
            finally
            {
                await receiver.CloseAsync();
            }
        }

        // Equivalent to Receive event, perform read & delete from topic 
        public async Task ProcessTopicMessageAsync(string subscriptionName)
        {
            var processor = _topicClient.CreateProcessor(_serviceBusOption.Value.Topic, subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += TopicMessageHandler;
                processor.ProcessErrorAsync += TopicErrorHandler;

                await processor.StartProcessingAsync();
                Thread.Sleep(30 * 1000);
                await processor.StopProcessingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service bus topic message receive failed. Error: {ex}");
            }
            finally
            {
                await processor.CloseAsync();
                await _topicClient.DisposeAsync();
            }
        }

        public async Task GetSubscriptionsAsync()
        {
            try
            {
                var subscriptionNames = new List<string>();
                var subscriptions = _topicAdminClient.GetSubscriptionsAsync(_serviceBusOption.Value.Topic);
                
                await foreach (var item in subscriptions)
                {
                    subscriptionNames.Add(item.SubscriptionName);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region private methods

        #region Queue
        private ServiceBusClient GetQueueClient()
        {
            var client = new ServiceBusClient(_serviceBusOption.Value.QueueConnectionString);
            return client;
        }

        private static async Task QueueMessageHandler(ProcessMessageEventArgs args)
        {
            var jsonString = Encoding.UTF8.GetString(args.Message.Body);
            await args.CompleteMessageAsync(args.Message); // Read and delete from queue
            Console.WriteLine($"Message received from service bus queue: {jsonString}");
        }

        private static Task QueueErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Service bus queue message receive failed. Error: {args.Exception}");
            return Task.CompletedTask;
        }
        #endregion

        #region topic
        private ServiceBusClient GetTopicClient()
        {
            var client = new ServiceBusClient(_serviceBusOption.Value.TopicConnectionString);
            return client;
        }

        private static async Task TopicMessageHandler(ProcessMessageEventArgs args)
        {
            var jsonString = Encoding.UTF8.GetString(args.Message.Body);
            await args.CompleteMessageAsync(args.Message); // Read and delete from topic
            Console.WriteLine($"Message received from service bus topic: {jsonString}");
        }

        private static Task TopicErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Service bus topic message receive failed. Error: {args.Exception}");
            return Task.CompletedTask;
        }
        #endregion

        #endregion
    }
}
