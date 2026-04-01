using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<List<Project>> GetProjectByUser(string userId);
        Task<bool> CheckExistProject(string projectCode);
        Task<Project> GetProjectInformation(int id);
        Task<Project> GetProjectById(int id);
        Task<List<Project>> GetSearchProjectByUser(string userId, string keyWord);
        Task SaveChangeAsync();
    }
}
