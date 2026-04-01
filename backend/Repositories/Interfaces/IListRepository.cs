using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface IListRepository
    {
        Task AddAsync(Lists list);
        Task<Lists> GetListById(int listId);
        Task<bool> CheckListExistInProject(string name, int projectId);
        Task<List<Lists>> GetAllListsByProject(int projectId);
        Task<List<Lists>> GetAllListsInRange(int start, int end, int projectId);
        Task<List<Tasks>> GetTaskInSection(int listId);
        Task<List<Lists>> GetSectionListTaskById(int projectId);
        Task<List<Lists>> GetListOrderByIndex();
        Task<List<Lists>> GetSectionsByProject(int projectId);
        Task SaveChangeAsync();
    }
}
