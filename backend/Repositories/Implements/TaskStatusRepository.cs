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
    public class TaskStatusRepository : ITaskStatusRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskStatusRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskStatuses>> GetAllTaskStatus()
        {
            try
            {
                return await _dbContext.TaskStatus.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
