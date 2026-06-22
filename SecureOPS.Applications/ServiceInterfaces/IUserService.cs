using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
    public interface IUserService
    {
        Task<bool> HasUserReceivedNotificationAsync(int userId, string messageContent);

        Task NotifyRoleAsync(string message, string role);
        Task<User?> GetByIdAsync(int userId);
    }
}
