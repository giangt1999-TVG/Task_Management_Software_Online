using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface IFileAttachmentRepository
    {
        Task AddAsync(FileAttachment file);
        Task<FileAttachment> GetFileAttachment(int id);
        Task<FileAttachment> GetFileAttachmentTaskById(int taskId);
        Task SaveChangeAsync();
    }
}
