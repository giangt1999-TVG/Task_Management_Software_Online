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
    public class TaskPriorityRepository : ITaskPriorityRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskPriorityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskPriority>> GetAllTaskPriority()
        {
            try
            {
                return await _dbContext.TaskPriority.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
