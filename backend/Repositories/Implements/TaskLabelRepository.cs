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
    public class TaskLabelRepository : ITaskLabelRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskLabelRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TaskLabel taskLabel)
        {
            try
            {
                await _dbContext.TaskLabel.AddAsync(taskLabel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TaskLabel> GetTaskLabelByIdAsync(int taskId)
        {
            try
            {
                return await _dbContext.TaskLabel.Where(t => t.TaskId == taskId).FirstOrDefaultAsync();
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
