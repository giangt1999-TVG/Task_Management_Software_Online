using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface IChecklistRepository
    {
        Task AddAsync(Checklist checklist);
        Task<Checklist> GetChecklistByIdAsync(int checklistId);
        Task SaveChangeAsync();
    }
}
