using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface IFbUserTokenRepository
    {
        Task AddAsync(FbUserToken fbUserToken);
        Task<List<FbUserToken>> GetTokensByUserIds(List<string> userIds);
        Task<bool> CheckTokenExisted(string token);
        Task SaveChangeAsync();
    }
}
