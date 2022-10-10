using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IDataLakeStorageService
    {
        public Task<DataLakeDirectoryClient> CreateDirectory(string directory);
        Task UploadFile(string directory, string file);
        Task UploadLargeFile(string directory, string file);
        Task DownloadFile(string directory, string file);
        Task<List<string>> ListFilesInDirectory(string directory);
    }
}
