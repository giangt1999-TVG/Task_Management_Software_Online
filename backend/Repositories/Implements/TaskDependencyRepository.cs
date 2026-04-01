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
    public class TaskDependencyRepository : ITaskDependencyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskDependencyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TaskDependency taskDependency)
        {
            try
            {
                await _dbContext.TaskDependency.AddAsync(taskDependency);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<TaskDependency> GetTaskDependencyByIdAsync(int taskId)
        {
            try
            {
                return await _dbContext.TaskDependency.Where(t => t.TaskDependencyId == taskId).FirstOrDefaultAsync();
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
