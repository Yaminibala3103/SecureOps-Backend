using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;

        public UserService(IUserRepository userRepo) => _userRepo = userRepo;

        public async Task<bool> HasUserReceivedNotificationAsync(int userId, string messageContent)
        {
            var user = await _userRepo.GetUserWithNotificationsAsync(userId);
            if (user == null) return false;

            // Checks if any notification assigned to this user matches the specific alert message
            return user.Notifications.Any(n => n.Message.Contains(messageContent));
        }

        // You'll need to inject INotificationService or INotificationRepository here too
        public async Task NotifyRoleAsync(string message, string role)
        {
            var usersInRole = await _userRepo.GetUsersByRoleAsync(role);

            foreach (var user in usersInRole)
            {
                // This triggers both the SQL save and the SignalR push for each user in that role
                await _notificationService.CreateSystemNotificationAsync(message, "Role-Based Alert", user.UserId);
            }
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            // Business logic can be added here if needed
            return await _userRepo.GetByIdAsync(userId);
        }
    }

}
