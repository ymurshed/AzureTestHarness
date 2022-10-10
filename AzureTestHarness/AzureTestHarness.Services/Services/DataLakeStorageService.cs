using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using AzureTestHarness.Shared.Interfaces;
using AzureTestHarness.Shared.Models.OptionModels;
using Microsoft.Extensions.Options;

namespace AzureTestHarness.Services.Services
{
    public class DataLakeStorageService : IDataLakeStorageService
    {
        private readonly DataLakeStorageOption _dataLakeStorageOption;
        private readonly DataLakeFileSystemClient _fileSystemClient;

        public DataLakeStorageService(IOptions<DataLakeStorageOption> dataLakeStorageOption)
        {
            _dataLakeStorageOption = dataLakeStorageOption.Value;
            var client = GetDataLakeServiceClient();
            _fileSystemClient = client.GetFileSystemClient(_dataLakeStorageOption.Container);
        }

        public async Task<DataLakeDirectoryClient> CreateDirectory(string directory)
        {
            var directoryClient = await _fileSystemClient.CreateDirectoryAsync(directory);
            return directoryClient;
        }

        public async Task UploadFile(string directory, string file)
        {
            var directoryClient = _fileSystemClient.GetDirectoryClient(directory);
            var fileClient = await directoryClient.CreateFileAsync(Path.GetFileName(file));
            
            var fileStream = File.OpenRead(file);
            long fileSize = fileStream.Length;
            await fileClient.Value.AppendAsync(fileStream, offset: 0);
            await fileClient.Value.FlushAsync(position: fileSize);
        }

        public async Task UploadLargeFile(string directory, string file)
        {
            var directoryClient = _fileSystemClient.GetDirectoryClient(directory);
            var fileClient = directoryClient.GetFileClient(Path.GetFileName(file));

            var fileStream = File.OpenRead(file);
            await fileClient.UploadAsync(fileStream);
        }

        public async Task DownloadFile(string directory, string file)
        {
            var directoryClient = _fileSystemClient.GetDirectoryClient(directory);
            var fileClient = directoryClient.GetFileClient(Path.GetFileName(file));
            var downloadResponse = await fileClient.ReadAsync();
            var reader = new BinaryReader(downloadResponse.Value.Content);

            int count;
            var bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            var fileStream = File.OpenWrite(file);

            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
            {
                fileStream.Write(buffer, 0, count);
            }

            await fileStream.FlushAsync();
            fileStream.Close();
        }

        public async Task<List<string>> ListFilesInDirectory(string directory)
        {
            IAsyncEnumerator<PathItem> enumerator = _fileSystemClient.GetPathsAsync(directory).GetAsyncEnumerator();
            await enumerator.MoveNextAsync();
            PathItem item = enumerator.Current;
            var files = new List<string>();

            while (item != null)
            {
                files.Add(item.Name);

                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }

                item = enumerator.Current;
            }

            return files;
        }

        #region private methods
        private DataLakeServiceClient GetDataLakeServiceClient()
        {
            var sharedKeyCredential = new StorageSharedKeyCredential(_dataLakeStorageOption.AccountName, _dataLakeStorageOption.AccountKey);
            var client = new DataLakeServiceClient(new Uri(_dataLakeStorageOption.Uri), sharedKeyCredential);
            return client;
        }
        #endregion
    }
}
