using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace AzureTestHarness.Services.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IOptions<BlobStorageOption> _blobStorageOption;
        private readonly BlobServiceClient _client;

        public BlobStorageService(IOptions<BlobStorageOption> blobStorageOption)
        {
            _blobStorageOption = blobStorageOption;
            _client = GetClient();
        }

        #region Public
        public async Task<BlobContentInfo> UploadAsync(string filePath, string fileName)
        {
            BlobContentInfo blobContentInfo = null;

            try
            {
                var blobClient = GetBlobClient(fileName);
                if (blobClient == null) return null;
                blobContentInfo = await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = GetContentType(filePath) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BlobService upload failed. Error: {ex}");
            }
            return blobContentInfo;
        }

        public async Task<Azure.Response> DownloadAsync(string blobName, string downloadPath)
        {
            Azure.Response response = null;

            try
            {
                var blobClient = GetBlobClient(blobName);
                if (blobClient == null) return null;
                response = await blobClient.DownloadToAsync(downloadPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BlobService download failed. Error: {ex}");
            }
            return response;
        }

        public async Task<IEnumerable<string>> ListBlobsAsync()
        {
            var result = new List<string>();

            try
            {
                var containerClient = GetBlobContainerClient();
                if (containerClient == null) return result;
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    result.Add(blobItem.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BlobService list failed. Error: {ex}");
            }
            return result;
        }
        
        public async Task DeleteAsync(string blobName)
        {
            try
            {
                var blobClient = GetBlobClient(blobName);
                if (blobClient == null) return;
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BlobService deletion failed. Error: {ex}");
            }
        }
        #endregion

        #region private methods
        private BlobServiceClient GetClient()
        {
            var client = new BlobServiceClient(_blobStorageOption.Value.ConnectionString);
            return client;
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            try
            {
                return _client.GetBlobContainerClient(_blobStorageOption.Value.Container);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBlobContainerClient error: {ex}");
            }
            return null;
        }

        private BlobClient GetBlobClient(string blobName)
        {
            try
            {
                return GetBlobContainerClient()?.GetBlobClient(blobName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBlobClient error: {ex}");
            }
            return null;
        }

        private static string GetContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType)) contentType = "application/octet-stream";
            return contentType;
        }
        #endregion
    }
}
