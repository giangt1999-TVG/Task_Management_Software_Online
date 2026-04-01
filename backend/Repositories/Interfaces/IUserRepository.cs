using System.Collections.Generic;
using System.Threading.Tasks;
using tms_api.Models;
using tms_api.ViewModels.ProjectViewModel;
using tms_api.ViewModels.UserViewModel;

namespace tms_api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserRoleWithProject(string userId, string roleId, int projectId);
        //Task InitilizeUserAsync(ApplicationUser user);
        //Task<User> FindByUserName(string userName);
        //Task<List<ApplicationUser>> GetUsersWithUserName(IEnumerable<string> userNames);
        Task<List<User>> GetUserByRole(string roleName);
        Task<List<User>> GetUserInProject(int projectId);
        Task<Role> GetRoleByUserId(string userId, int projectId);
        Task<List<UserDto>> GetUserInfoInProject(int projectId);
        Task<List<AccountViewModel>> GetListUsersInformation();
        Task<List<AccountViewModel>> GetSearchAccount(string keyWord);
        Task RemoveProjectMember(string userId, int projectId);
        Task SaveChangeAsync();
    }
}
