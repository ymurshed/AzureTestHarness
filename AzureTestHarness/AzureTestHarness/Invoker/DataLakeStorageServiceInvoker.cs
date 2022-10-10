using System;
using System.Threading.Tasks;
using AzureTestHarness.Shared.Interfaces;

namespace AzureTestHarness.Invoker
{
    public class DataLakeStorageServiceInvoker : IInvoker
    {
        private readonly IDataLakeStorageService _dataLakeStorageService;

        public DataLakeStorageServiceInvoker(IDataLakeStorageService dataLakeStorageService)
        {
            Console.WriteLine("\nCalling Azure DataLake Storage Service --->>> ");
            _dataLakeStorageService = dataLakeStorageService;
        }

        public async Task Invoke()
        {
            var directory = "test-dir-new/sub-dir/child-dir";
            var smallFile = @"D:\data-lake test\small\patient.pdf";
            var largeFile = @"D:\data-lake test\large\test.msi";
            var downloadFile = @"D:\data-lake test\large-download\test.msi";

            await _dataLakeStorageService.CreateDirectory(directory);
            await _dataLakeStorageService.UploadFile(directory, smallFile);
            await _dataLakeStorageService.UploadLargeFile(directory, largeFile);
            var files = await _dataLakeStorageService.ListFilesInDirectory(directory);
            
            Console.WriteLine("Listing files from data lake storage: ");
            foreach (var file in files) Console.WriteLine(file);

            await _dataLakeStorageService.DownloadFile(directory, downloadFile);
        }
    }
}
