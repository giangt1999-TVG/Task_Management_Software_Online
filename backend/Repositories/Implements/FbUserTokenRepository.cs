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
    public class FbUserTokenRepository : IFbUserTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FbUserTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(FbUserToken fbUserToken)
        {
            try
            {
                await _dbContext.FbUserTokens.AddAsync(fbUserToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckTokenExisted(string token)
        {
            try
            {
                var tokenRecord = await _dbContext.FbUserTokens.Where(f => f.Token == token).FirstOrDefaultAsync();
                if (tokenRecord == null)
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

        public async Task<List<FbUserToken>> GetTokensByUserIds(List<string> userIds)
        {
            try
            {
                return await _dbContext.FbUserTokens.Where(f => userIds.Contains(f.UserId)).ToListAsync();
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
