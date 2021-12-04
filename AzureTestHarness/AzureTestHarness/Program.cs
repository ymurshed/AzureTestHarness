using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AzureTestHarness.Services;
using AzureTestHarness.Services.Services;
using AzureTestHarness.Shared.Interfaces;
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
                var keyVaultService = ServiceProvider.GetService<IKeyVaultService>();
                
                Task.Run(async () =>
                { 
                    var result = await keyVaultService.GetSecretAsync();
                    if (!string.IsNullOrEmpty(result))
                    {
                        var response = await keyVaultService.SetSecretAsync();
                    }
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        #region Private methods    
        public static void Configure()
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
            services.AddTransient<IKeyVaultService, KeyVaultService>();

            ServiceProvider = services.BuildServiceProvider();
        }
        #endregion
    }
}
