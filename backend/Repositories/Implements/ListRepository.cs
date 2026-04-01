using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Data;
using tms_api.Models;
using tms_api.Repositories.Interfaces;

namespace tms_api.Repositories.Implements
{
    public class ListRepository : IListRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ListRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Lists list)
        {
            try
            {
                await _dbContext.Lists.AddAsync(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckListExistInProject(string name, int projectId)
        {
            try
            {
                var list = await _dbContext.Lists.Where(l => l.Name.ToLower().Equals(name.ToLower()) && l.ProjectId == projectId).FirstOrDefaultAsync();

                if (list == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Lists>> GetAllListsByProject(int projectId)
        {
            try
            {
                return await _dbContext.Lists.Where(l => l.ProjectId == projectId).OrderBy(o => o.Index).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Lists>> GetAllListsInRange(int start, int end, int projectId)
        {
            try
            {
                return await _dbContext.Lists.Where(l => l.Index >= start && l.Index <= end && l.ProjectId == projectId).OrderBy(o => o.Index).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Lists> GetListById(int listId)
        {
            try
            {
                return await _dbContext.Lists.FindAsync(listId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Lists>> GetListOrderByIndex()
        {
            try
            {
                return await _dbContext.Lists.ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Lists>> GetSectionListTaskById(int projectId)
        {
            try
            {
                return await _dbContext.Lists.Where(li => li.ProjectId == projectId)
                    .Include(li => li.Tasks).ThenInclude(c => c.TaskLabel).ThenInclude(c => c.Label)
                                             .Include(li => li.Tasks)
                                                .ThenInclude(c => c.User).OrderBy(li => li.Index)
                                              .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Lists>> GetSectionsByProject(int projectId)
        {
            try
            {
                return await _dbContext.Lists.Where(l => l.ProjectId == projectId).OrderBy(o => o.Index).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Tasks>> GetTaskInSection(int listId)
        {
            try
            {
                return await _dbContext.Tasks.Where(x => x.ListId == listId).ToListAsync();
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


    }
}
