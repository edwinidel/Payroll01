using System.Collections.Generic;
using System.Threading.Tasks;
using _2FA.Models;

namespace _2FA.Services
{
    public interface INotificationService
    {
        Task<IReadOnlyList<NotificationDto>> GetAsync(int employeeId, bool unreadOnly, int page, int pageSize);
        Task MarkAsReadAsync(int employeeId, int notificationId);
        Task EnqueueAsync(NotificationDto notification);
    }
}
