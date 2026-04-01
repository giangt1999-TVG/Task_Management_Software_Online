using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Services.UploadFile
{
    public interface ICloudStorage
    {
        Task<string> UploadFileAsync(string bucketName, IFormFile file, string fileNameForStorage);
        Task DeleteFileAsync(string bucketName, string fileNameForStorage);
    }
}
