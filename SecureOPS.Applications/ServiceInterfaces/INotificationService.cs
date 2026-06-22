using SecureOPS.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId);
        Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto);

        // Watcher specific: Quick status updates for the React UI
        Task MarkAsReadAsync(int notificationId);
        Task CreateSystemNotificationAsync(string message, string category, int? userId = null);

        Task NotifyRoleAsync(string message, string category, string targetRole);
    }
}
