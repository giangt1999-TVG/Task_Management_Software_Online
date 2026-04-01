using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;
using tms_api.ViewModels.TaskViewModels;

namespace tms_api.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<Tasks>> GetListDueSoonTask(string userId, int daysDueIn);
        Task<Tasks> GetTaskByIdAsync(int taskId);
        Task<Tasks> GetTaskDetailByIdAsync(int taskId);
        Task<List<Tasks>> GetListSubtask(int taskId);
        Task AddAsync(Tasks task);
        Task<Tasks> GetTaskById(int taskId);
        Task<List<Tasks>> GetSubtaskByProject(int projectId);
        Task<List<ListViewViewModel>> GetTaskListView(int projectId);
        Task<List<Tasks>> GetListTask(int taskId);
        Task<List<MyTasksViewModel>> GetTasksByUser(string userId);
        Task<List<MyTasksViewModel>> GetSearchTasksOfUser(string userId, string keyWord);
        Task<List<MyTasksViewModel>> GetListSearchTMyTask(string userId, string keyWord);
        Task<List<Tasks>> GetAllTaskInProject(int projectId);
        Task SaveChangeAsync();
    }
}
