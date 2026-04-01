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
    public class DependencyRepository : IDependencyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DependencyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Dependency dependency)
        {
            try
            {
                await _dbContext.Dependency.AddAsync(dependency);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> CheckExistDependency(string name)
        {
            try
            {
                var dependency = await _dbContext.Dependency.Where(d => d.Name.ToLower().Equals(name.ToLower())).FirstOrDefaultAsync();
                if (dependency != null)
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

        public async Task<List<Dependency>> GetAllDependencies()
        {
            try
            {
                return await _dbContext.Dependency.ToListAsync();
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
