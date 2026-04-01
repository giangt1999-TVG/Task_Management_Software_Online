using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface ITaskStatusRepository
    {
        Task<List<TaskStatuses>> GetAllTaskStatus();
    }
}
