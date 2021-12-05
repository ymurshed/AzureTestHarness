using System;
using System.IO;
using System.Threading.Tasks;
using AzureTestHarness.Shared.Interfaces;

namespace AzureTestHarness.Invoker
{
    public class BlobStorageServiceInvoker : IInvoker
    {
        private readonly IBlobStorageService _blobStorageService;

        public BlobStorageServiceInvoker(IBlobStorageService blobStorageService)
        {
            Console.WriteLine("\nCalling Azure Blob Storage Service --->>> ");
            _blobStorageService = blobStorageService;
        }

        public async Task Invoke()
        {
            const string downloadPath = @"C:\Test\Download\Test 2.xlsx";
            const string file1Path = @"C:\Test\Test 1.docx";
            const string file2Path = @"C:\Test\Test 2.xlsx";

            await _blobStorageService.UploadAsync(file1Path, Path.GetFileName(file1Path));
            await _blobStorageService.UploadAsync(file2Path, Path.GetFileName(file2Path));
            await _blobStorageService.DownloadAsync("Test 2.xlsx", downloadPath);
            await _blobStorageService.DeleteAsync("Test 2.xlsx");
            var files = await _blobStorageService.ListBlobsAsync();
            
            Console.WriteLine("Listing files from blob storage: ");
            foreach (var file in files) Console.WriteLine(file);
        }
    }
}
