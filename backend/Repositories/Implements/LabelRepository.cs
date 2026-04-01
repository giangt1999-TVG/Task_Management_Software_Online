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
    public class LabelRepository : ILabelRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LabelRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Label label)
        {
            try
            {
                await _dbContext.Label.AddAsync(label);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckLabelExistInProject(string name, int projectId)
        {
            try
            {
                var label = await _dbContext.Label.Where(l => l.Name.ToLower().Equals(name.ToLower()) && l.ProjectId == projectId).FirstOrDefaultAsync();

                if (label == null)
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

        public async Task<Label> GetLabel(int labelId, int projectId)
        {
            try
            {
                return await _dbContext.Label.Where(l => l.LabelId == labelId && l.ProjectId == projectId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Label>> GetLabelInProject(int projectId)
        {
            try
            {
                return await _dbContext.Label.Where(x => x.ProjectId == projectId).ToListAsync();
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
