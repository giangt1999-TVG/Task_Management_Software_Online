using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;
using tms_api.ViewModels.NotificationViewModel;

namespace tms_api.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddRangeAsync(List<UserNotification> notifications);
        Task AddAsync(Notification notification);
        Task AddAsync(UserNotification notification);
        Task<List<UserNotification>> GetNotificationByUserId(string userId);
        Task<List<UserNotification>> GetUnReadNotificationByUserId(string userId);
        Task SaveChangeAsync();
    }
}
