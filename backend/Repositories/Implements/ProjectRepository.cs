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
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProjectRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Project project)
        {
            try
            {
                await _dbContext.Project.AddAsync(project);
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

        public async Task<List<Project>> GetProjectByUser(string userId)
        {
            try
            {
                return await _dbContext.Project
                                .Join(_dbContext.ProjectMember, p => p.ProjectId, pm => pm.ProjectId, (p, pm) => new { p, pm })
                                .Join(_dbContext.Users, ppm => ppm.pm.UserId, u => u.Id, (ppm, u) => new { ppm, u })
                                .Where(ppmu => ppmu.u.Id == userId) 
                                .Select(ppmu => ppmu.ppm.p)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckExistProject(string projectCode)
        {
            try
            {
                var project = await _dbContext.Project.Where(x => x.ProjectCode.ToLower().Equals(projectCode.ToLower())).FirstOrDefaultAsync();

                if (project != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Project> GetProjectInformation(int id)
        {
            try
            {
                return await _dbContext.Project.Where(p => p.ProjectId == id)
                    .Include(p => p.ProjectMembers)
                        .ThenInclude(u => u.User)
                    .Include(p => p.Label)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Project>> GetSearchProjectByUser(string userId, string keyWord)
        {
            try
            {
                return await _dbContext.Project
                                .Join(_dbContext.ProjectMember, p => p.ProjectId, pm => pm.ProjectId, (p, pm) => new { p, pm })
                                .Join(_dbContext.Users, ppm => ppm.pm.UserId, u => u.Id, (ppm, u) => new { ppm, u })
                                .Where(ppmu => ppmu.u.Id == userId && (ppmu.ppm.p.Name.Contains(keyWord) || ppmu.ppm.p.ProjectCode.Contains(keyWord)))
                                .Select(ppmu => ppmu.ppm.p)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Project> GetProjectById(int id)
        {
            try
            {
                return await _dbContext.Project.Where(p => p.ProjectId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
