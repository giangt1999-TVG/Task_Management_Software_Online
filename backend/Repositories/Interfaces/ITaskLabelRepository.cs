using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface ITaskLabelRepository
    {
        Task AddAsync(TaskLabel taskLabel);
        Task<TaskLabel> GetTaskLabelByIdAsync(int taskId);
        Task SaveChangeAsync();
    }
}
