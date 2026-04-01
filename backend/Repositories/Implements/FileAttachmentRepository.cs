using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Data;
using tms_api.Models;
using tms_api.Repositories.Interfaces;

namespace tms_api.Repositories.Implements
{
    public class FileAttachmentRepository : IFileAttachmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FileAttachmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(FileAttachment file)
        {
            try
            {
                await _dbContext.FileAttachment.AddAsync(file);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FileAttachment> GetFileAttachment(int id)
        {
            try
            {
                return await _dbContext.FileAttachment.Where(f => f.FileAttachmentId == id && f.IsDeleted == false).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<FileAttachment> GetFileAttachmentTaskById(int taskId)
        {
            try
            {
                return await _dbContext.FileAttachment.Where(f => f.TaskId == taskId && f.IsDeleted == false).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task SaveChangeAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
