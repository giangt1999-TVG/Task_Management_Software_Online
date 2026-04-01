using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Data;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.ViewModels.ProjectViewModel;
using tms_api.ViewModels.UserViewModel;

namespace tms_api.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserRoleWithProject(string userId, string roleId, int projectId)
        {
            try
            {
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    ProjectId = projectId
                };

                await _dbContext.UserRoles.AddAsync(userRole);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<AccountViewModel>> GetListUsersInformation()
        {
            try
            {
                var accounts = await (from u in _dbContext.Users
                                   join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                   join role in _dbContext.Roles on ur.RoleId equals role.Id
                                   where  ur.ProjectId == null && u.IsDeleted == false
                                   select new AccountViewModel
                                   {
                                       Id = u.Id,
                                       UserName=u.UserName,
                                       FullName = u.FullName,
                                       Email=u.Email,
                                       Role = role.Name,
                                       RollNumber=u.RollNumber
                                   }).ToListAsync();

                return accounts;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Role> GetRoleByUserId(string userId, int projectId)
        {
            try
            {
                var roleAccount = await (from ur in _dbContext.UserRoles
                                      join role in _dbContext.Roles on ur.RoleId equals role.Id
                                      where ur.ProjectId == projectId && ur.UserId == userId
                                      select role).FirstOrDefaultAsync();

                return roleAccount;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<AccountViewModel>> GetSearchAccount(string keyWord)
        {
            try
            {
                var users = await (from u in _dbContext.Users
                                   join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                   join r in _dbContext.Roles on ur.RoleId equals r.Id
                                   where ur.ProjectId==null && u.IsDeleted == false && (u.FullName.Contains(keyWord) || u.UserName.Contains(keyWord))
                                   select new AccountViewModel
                                   {
                                       Id=u.Id,
                                       UserName=u.UserName,
                                       FullName=u.FullName,
                                       Email=u.Email,
                                       Role=r.Name,
                                       RollNumber=u.RollNumber
                                   }).ToListAsync();
                return users;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //điều kiện lấy role Name, project id = null 
        public async Task<List<User>> GetUserByRole(string roleName)
        {
            try
            {
                var users = await (from u in _dbContext.Users
                                   join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                   join role in _dbContext.Roles on ur.RoleId equals role.Id
                                   where role.Name == roleName && ur.ProjectId == null && u.IsDeleted == false
                                   select u).ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<UserDto>> GetUserInfoInProject(int projectId)
        {
            try
            {
                var users = await (from u in _dbContext.Users
                                  join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                  join r in _dbContext.Roles on ur.RoleId equals r.Id
                                  where ur.ProjectId == projectId
                                  select new UserDto { 
                                    Id = u.Id,
                                    FullName = u.FullName,
                                    AvatarUrl = u.AvatarUrl,
                                    Role = r.Name
                                  }).ToListAsync();
                return users;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<User>> GetUserInProject(int projectId)
        {
            try
            {
                var users = await (from u in _dbContext.Users
                                   join um in _dbContext.ProjectMember on u.Id equals um.UserId
                                   join p in _dbContext.Project on um.ProjectId equals p.ProjectId
                                   where p.ProjectId == projectId && u.IsDeleted == false
                                   select u).ToListAsync();
                return users;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task RemoveProjectMember(string userId, int projectId)
        {
            try
            {
                _dbContext.Database.ExecuteSqlRaw("DELETE From UserRoles Where UserId = {0} AND ProjectId = {1}", userId, projectId);

                return Task.CompletedTask;
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
