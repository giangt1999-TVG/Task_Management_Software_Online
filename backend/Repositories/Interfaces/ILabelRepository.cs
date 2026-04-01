using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface ILabelRepository
    {
        Task<List<Label>> GetLabelInProject(int projectId);
        Task AddAsync(Label label);
        Task<bool> CheckLabelExistInProject(string name, int projectId);
        Task<Label> GetLabel(int labelId, int projectId);
        Task SaveChangeAsync();
    }
}
