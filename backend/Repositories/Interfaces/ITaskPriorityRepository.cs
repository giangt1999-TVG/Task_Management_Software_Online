using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface ITaskPriorityRepository
    {
        Task<List<TaskPriority>> GetAllTaskPriority();
    }
}
