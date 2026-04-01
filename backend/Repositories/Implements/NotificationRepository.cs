using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Data;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.ViewModels.NotificationViewModel;

namespace tms_api.Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NotificationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Notification notification)
        {
            try
            {
                await _dbContext.Notification.AddAsync(notification);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddAsync(UserNotification notification)
        {
            try
            {
                await _dbContext.UserNotification.AddAsync(notification);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddRangeAsync(List<UserNotification> notifications)
        {
            try
            {
                await _dbContext.UserNotification.AddRangeAsync(notifications);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserNotification>> GetNotificationByUserId(string userId)
        {
            try
            {
                return await _dbContext.UserNotification.Where(u => u.UserId == userId)
                        .Include(un => un.Notification)
                            .OrderByDescending(un => un.CreatedDate)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserNotification>> GetUnReadNotificationByUserId(string userId)
        {
            try
            {
                return await _dbContext.UserNotification.Where(u => u.UserId == userId && u.IsViewed == false).ToListAsync();
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
