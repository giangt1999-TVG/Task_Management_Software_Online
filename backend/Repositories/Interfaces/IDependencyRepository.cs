using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Repositories.Interfaces
{
    public interface IDependencyRepository
    {
        Task AddAsync(Dependency dependency);
        Task<List<Dependency>> GetAllDependencies();
        Task<bool> CheckExistDependency(string name);
       
        Task SaveChangeAsync();
    }
}
