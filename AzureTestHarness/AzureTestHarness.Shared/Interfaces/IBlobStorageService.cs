using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace AzureTestHarness.Shared.Interfaces
{
    public interface IBlobStorageService
    {
        Task<BlobContentInfo> UploadAsync(string filePath, string fileName);
        Task<Azure.Response> DownloadAsync(string blobName, string downloadPath);
        Task<IEnumerable<string>> ListBlobsAsync();
        Task DeleteAsync(string blobName);
    }
}
