using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctionAppDemo
{
    public static class BlobFunction
    {
        [FunctionName("BlobFunction")]
        public static void Run([BlobTrigger("harnessbs/{name}", Connection = "AzureWebJobsStorage")]Stream inputBlob, 
                               [Blob("output/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")]Stream outputBlob,
                               string name, 
                               ILogger log)
        {
            inputBlob.CopyTo(outputBlob);
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");
        }
    }
}
