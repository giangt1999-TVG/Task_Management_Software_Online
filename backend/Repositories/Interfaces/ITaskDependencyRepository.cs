using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface ITaskDependencyRepository
    {
        Task AddAsync(TaskDependency taskDependency);
        Task<TaskDependency> GetTaskDependencyByIdAsync(int taskId);
        Task SaveChangeAsync();
    }
}
