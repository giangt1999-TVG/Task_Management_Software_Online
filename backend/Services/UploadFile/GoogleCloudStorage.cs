using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Services.UploadFile
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly StorageClient _storageClient;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            _storageClient = StorageClient.Create();
        }
        public async Task DeleteFileAsync(string bucketName, string fileNameForStorage)
        {
            await _storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }

        public async Task<string> UploadFileAsync(string bucketName, IFormFile file, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var dataObject = await _storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
                return dataObject.MediaLink;
            }
        }
    }
}
