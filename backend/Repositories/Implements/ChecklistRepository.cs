using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Data;
using tms_api.Models;
using tms_api.Repositories.Interfaces;

namespace tms_api.Repositories.Implements
{
    public class ChecklistRepository : IChecklistRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ChecklistRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Checklist checklist)
        {
            try
            {
                await _dbContext.Checklist.AddAsync(checklist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Checklist> GetChecklistByIdAsync(int checklistId)
        {
            try
            {
                return await _dbContext.Checklist.FindAsync(checklistId);
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
