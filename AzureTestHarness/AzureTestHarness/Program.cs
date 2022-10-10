using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AzureTestHarness.Invoker;
using AzureTestHarness.Services.Services;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.Constants;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTestHarness
{
    public class Program
    {
        public static IServiceProvider ServiceProvider;
        public static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            try
            {
                Configure();

                var services = new List<Enums.AzureService>
                {
                    //Enums.AzureService.KeyVault,
                    //Enums.AzureService.BlobStorage,
                    //Enums.AzureService.ServiceBus,
                    Enums.AzureService.DataLakeStorage,
                };

                Task.Run(async () =>
                {
                    foreach (var service in services)
                    {
                        switch (service)
                        {
                            case Enums.AzureService.KeyVault:
                            var kv = new KeyVaultServiceInvoker(ServiceProvider.GetService<IKeyVaultService>());
                            await kv.Invoke();
                            break;

                            case Enums.AzureService.BlobStorage:
                            var bs = new BlobStorageServiceInvoker(ServiceProvider.GetService<IBlobStorageService>());
                            await bs.Invoke();
                            break;

                            case Enums.AzureService.ServiceBus:
                            var sb = new ServiceBusServiceInvoker(ServiceProvider.GetService<IServiceBusService>());
                            await sb.Invoke();
                            break;

                            case Enums.AzureService.DataLakeStorage:
                            var dl = new DataLakeStorageServiceInvoker(ServiceProvider.GetService<IDataLakeStorageService>());
                            await dl.Invoke();
                            break;

                            default: throw new ArgumentOutOfRangeException();
                        }
                    }

                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        #region Private methods    
        private static void Configure()
        {
            #region Set startup path
            var appBasePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            Console.WriteLine("App Path: " + appBasePath);

            Configuration = new ConfigurationBuilder()
                .SetBasePath(appBasePath)
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();
            #endregion

            var services = new ServiceCollection().AddOptions();
            
            services.Configure<KeyVaultOption>(Configuration.GetSection(nameof(KeyVaultOption)));
            services.Configure<BlobStorageOption>(Configuration.GetSection(nameof(BlobStorageOption)));
            services.Configure<ServiceBusOption>(Configuration.GetSection(nameof(ServiceBusOption)));
            services.Configure<DataLakeStorageOption>(Configuration.GetSection(nameof(DataLakeStorageOption)));

            services.AddTransient<IKeyVaultService, KeyVaultService>();
            services.AddTransient<IBlobStorageService, BlobStorageService>();
            services.AddTransient<IServiceBusService, ServiceBusService>();
            services.AddTransient<IDataLakeStorageService, DataLakeStorageService>();

            ServiceProvider = services.BuildServiceProvider();
        }
        #endregion
    }
}
