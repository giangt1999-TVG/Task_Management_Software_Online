using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Data;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.ViewModels.TaskViewModels;

namespace tms_api.Repositories.Implements
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Tasks task)
        {
            try
            {
                await _dbContext.Tasks.AddAsync(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<Tasks>> GetListDueSoonTask(string userId, int daysDueIn)
        {
            try
            {
                var now = DateTime.Now;
                return await _dbContext.Tasks.Where(t => t.UserId == userId && t.DueDate <= now.AddDays(daysDueIn) && t.IsActive == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Tasks>> GetListSubtask(int taskId)
        {
            try
            {
                return await _dbContext.Tasks.Where(t => t.ParentId == taskId)
                                .Include(t => t.TaskStatus)
                                .Include(t => t.List)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tasks> GetTaskByIdAsync(int taskId)
        {
            try
            {
                return await _dbContext.Tasks.Where(t => t.TaskId == taskId && t.IsActive).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tasks> GetTaskDetailByIdAsync(int taskId)
        {
            try
            {
                return await _dbContext.Tasks.Where(t => t.TaskId == taskId && t.IsActive)
                        .Include(t => t.TaskPriority)
                        .Include(t => t.TaskStatus)
                        .Include(t => t.List)
                            .ThenInclude(l => l.Project)
                        .Include(t => t.User)
                        .Include(t => t.Comment)
                            .ThenInclude(c => c.User)
                        .Include(t => t.Checklist)
                        .Include(t => t.TaskDependencies)
                            .ThenInclude(td => td.Dependency)
                        .Include(t => t.TaskLabel)
                            .ThenInclude(tl => tl.Label)
                        .Include(t => t.FileAttachments)
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tasks> GetTaskById(int taskId)
        {
            try
            {
                return await _dbContext.Tasks.Include(t => t.TaskStatus).Where(t => t.TaskId == taskId && t.IsActive).FirstOrDefaultAsync();
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

        public async Task<List<Tasks>> GetSubtaskByProject(int projectId)
        {
            try
            {
                var tasks = await (from p in _dbContext.Project
                                   join l in _dbContext.Lists on p.ProjectId equals l.ProjectId
                                   join t in _dbContext.Tasks on l.ListId equals t.ListId
                                   where p.ProjectId == projectId && p.IsDeleted == false
                                   && l.IsDeleted == false && t.IsDeleted == false && t.ParentId == null
                                   select t).ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<ListViewViewModel>> GetTaskListView(int projectId)
        {
            try
            {
              
                var tasks = await _dbContext.Tasks
                    .Include(t => t.User)
                    .Include(t => t.TaskStatus)
                    .Include(t=>t.List)
                    .Include(t=>t.TaskLabel)
                     .ThenInclude(t => t.Label)
                     .ThenInclude(c => c.Project).Where(t => t.List.Project.ProjectId == projectId)
                     .OrderBy(t => t.ParentId == null ? t.TaskId : t.ParentId)
                     .ThenBy(t => t.ParentId == null ? 0 : 1)
                     .Select(k =>new ListViewViewModel{
                         TaskId = k.TaskId,
                         Name = k.Name,
                         DueDate = k.DueDate,
                         Category = (k.ParentId == null || k.ParentId == 0) ? "Task" : "Subtask",
                         Assignee = (k.User != null) ? new AssigneeViewModelTask { UserId = k.UserId, AvatarUrl = k.User.AvatarUrl, FullName = k.User.FullName } : null,
                         TaskStatus = (k.TaskStatus != null) ? new TaskStatusViewModel { TaskStatusId = k.TaskStatus.TaskStatusId, Name = k.TaskStatus.Name } : null,
                         Section = (k.List != null) ? new SectionInListViewModel { ListId = k.List.ListId, Name = k.List.Name } : null,
                         ListLabel = k.TaskLabel.Select(labelnew => new LabelViewModel
                         {
                             LabelId= labelnew.Label.LabelId,
                             Name= labelnew.Label.Name,
                             Color= labelnew.Label.Color
                         }).ToList()
                     })
                    .ToListAsync();
                return tasks;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public async Task<List<Tasks>> GetListTask(int taskId)
        {
            try
            {
                return await _dbContext.Tasks.Where(t => t.TaskId == taskId).ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<MyTasksViewModel>> GetTasksByUser(string userId)
        {
            try
            {

                var tasks = await (from t in _dbContext.Tasks
                                   join l in _dbContext.Lists on t.ListId equals l.ListId
                                   join p in _dbContext.Project on l.ProjectId equals p.ProjectId
                                   join u in _dbContext.Users on t.UserId equals u.Id
                                   join ts in _dbContext.TaskStatus on t.TaskStatusId equals ts.TaskStatusId
                                   where t.UserId == userId && p.IsDeleted == false && t.IsDeleted == false && ts.IsDeleted == false
                                   select new MyTasksViewModel
                                   {
                                       TaskId = t.TaskId,
                                       Name = t.Name,
                                       DueDate = t.DueDate,
                                       Category = (t.ParentId == null || t.ParentId == 0) ? "Task" : "Subtask",
                                       Assignee = (t.User != null) ? new AssigneeViewModelTask { UserId = u.Id, AvatarUrl = u.AvatarUrl, FullName = u.FullName } : null,
                                       TaskStatus = (t.TaskStatus != null) ? new TaskStatusViewModel { TaskStatusId = ts.TaskStatusId, Name = ts.Name } : null,
                                       ProjectName = p.Name,
                                       ProjectId = p.ProjectId
                                   }).ToListAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MyTasksViewModel>> GetSearchTasksOfUser(string userId, string keyWord)
        {
            try
            {

                var tasks = await (from t in _dbContext.Tasks
                                   join l in _dbContext.Lists on t.ListId equals l.ListId
                                   join p in _dbContext.Project on l.ProjectId equals p.ProjectId
                                   join pm in _dbContext.ProjectMember on p.ProjectId equals pm.ProjectId
                                   join u in _dbContext.Users on pm.UserId equals u.Id
                                   join ts in _dbContext.TaskStatus on t.TaskStatusId equals ts.TaskStatusId
                                   where u.Id == userId && p.IsDeleted == false && t.IsDeleted == false && ts.IsDeleted == false && t.Name.Contains(keyWord)
                                   select new MyTasksViewModel
                                   {
                                       TaskId = t.TaskId,
                                       Name = t.Name,
                                       DueDate = t.DueDate,
                                       Assignee = (t.User != null) ? new AssigneeViewModelTask { UserId = u.Id, AvatarUrl = u.AvatarUrl, FullName = u.FullName } : null,
                                       TaskStatus = (t.TaskStatus != null) ? new TaskStatusViewModel { TaskStatusId = ts.TaskStatusId, Name = ts.Name } : null,
                                       ProjectName = p.Name,
                                   }).ToListAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Tasks>> GetAllTaskInProject(int projectId)
        {
            try
            {

                var tasks = await (from p in _dbContext.Project
                                  join l in _dbContext.Lists on p.ProjectId equals l.ProjectId
                                  join t in _dbContext.Tasks on l.ListId equals t.ListId
                                  where p.ProjectId == projectId && !p.IsDeleted && !l.IsDeleted && !t.IsDeleted
                                  select t).ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MyTasksViewModel>> GetListSearchTMyTask(string userId, string keyword)
        {
            try
            {
                var tasks = await _dbContext.Tasks
                     .Include(t => t.TaskStatus)
                    .Include(t => t.List)
                    .ThenInclude(c => c.Project)
                    .Include(t => t.User).Where(t => t.UserId == userId && t.Name.Contains(keyword))
                    .Select(k => new MyTasksViewModel
                    {
                        TaskId = k.TaskId,
                        Name = k.Name,
                        DueDate = k.DueDate,
                        Category = (k.ParentId == null || k.ParentId == 0) ? "Task" : "Subtask",
                        Assignee = (k.User != null) ? new AssigneeViewModelTask { UserId = k.UserId, AvatarUrl = k.User.AvatarUrl, FullName = k.User.FullName } : null,
                        TaskStatus = (k.TaskStatus != null) ? new TaskStatusViewModel { TaskStatusId = k.TaskStatus.TaskStatusId, Name = k.TaskStatus.Name } : null,
                        ProjectName = k.Name
                    }).ToListAsync();
                return tasks;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
