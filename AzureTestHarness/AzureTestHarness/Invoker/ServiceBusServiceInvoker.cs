using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.DataModel;

namespace AzureTestHarness.Invoker
{
    public class ServiceBusServiceInvoker : IInvoker
    {
        private readonly IServiceBusService _serviceBusService;

        public ServiceBusServiceInvoker(IServiceBusService serviceBusService)
        {
            Console.WriteLine("\nCalling Service Bus Service --->>> ");
            _serviceBusService = serviceBusService;
        }

        public async Task Invoke()
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    Name = "Yaad Murshed",
                    Age = 33,
                    Address = "406 Dilu Road"
                },
                new Employee
                {
                    Name = "Asif Ahmed",
                    Age = 33,
                    Address = "14 Mirpur"
                }
            };

            await _serviceBusService.SendMessageAsync(employees.FirstOrDefault());
            await _serviceBusService.SendMessageAsync(employees.LastOrDefault());
            var msg = await _serviceBusService.ReceiveMessageAsync();
            Console.WriteLine($"Message peeked: {msg}");
            await _serviceBusService.ProcessMessageAsync();
        }
    }
}
