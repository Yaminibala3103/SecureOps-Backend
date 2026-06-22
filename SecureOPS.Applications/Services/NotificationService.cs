using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Applications.Exceptions; // Use custom exceptions
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureOPS.Applications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notifRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            INotificationRepository notifRepo,
            IUserRepository userRepo,
            IMapper mapper,
            IHubContext<NotificationHub> hubContext)
        {
            _notifRepo = notifRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var entities = await _notifRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(entities);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
        {
            // Verify user exists before fetching notifications
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User {userId} not found.");

            var entities = await _notifRepo.GetAllAsync();
            var userNotifs = entities.Where(n => n.UserId == userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(userNotifs);
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationDto dto)
        {
            if (dto == null)
                throw new AppValidationException("Notification data is required.");

            try
            {
                var entity = _mapper.Map<Notification>(dto);
                entity.Status = "Unread";
                entity.CreatedDate = DateTime.Now;

                var result = await _notifRepo.AddAsync(entity);

                if (result.UserId.HasValue)
                {
                    await _hubContext.Clients.User(result.UserId.Value.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            message = result.Message,
                            category = result.Category,
                            timestamp = result.CreatedDate
                        });
                }

                return _mapper.Map<NotificationDto>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create and send notification.", ex);
            }
        }

        public async Task CreateSystemNotificationAsync(string message, string category, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new AppValidationException("System notification message cannot be empty.");

            var entity = new Notification
            {
                Message = message,
                Category = category,
                UserId = userId,
                Status = "Unread",
                CreatedDate = DateTime.Now
            };

            await _notifRepo.AddAsync(entity);

            try
            {
                if (userId.HasValue)
                {
                    await _hubContext.Clients.User(userId.Value.ToString())
                        .SendAsync("ReceiveNotification", message);
                }
                else
                {
                    // Broadcast to everyone if no specific user is provided
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
                }
            }
            catch (Exception ex)
            {
                // We don't throw here to ensure the record in DB persists
                Console.WriteLine($"SignalR Broadcast Warning: {ex.Message}");
            }
        }

        public async Task NotifyRoleAsync(string message, string category, string targetRole)
        {
            if (string.IsNullOrWhiteSpace(targetRole))
                throw new AppValidationException("Target role must be specified for escalation.");

            var users = await _userRepo.GetUsersByRoleAsync(targetRole);

            // If no users found, we don't throw an error, but we log it
            if (!users.Any())
            {
                Console.WriteLine($"Notification skip: No users found with role {targetRole}.");
                return;
            }

            foreach (var user in users)
            {
                try
                {
                    var entity = new Notification
                    {
                        Message = message,
                        Category = category,
                        UserId = user.UserId,
                        Status = "Unread",
                        CreatedDate = DateTime.Now
                    };

                    await _notifRepo.AddAsync(entity);

                    await _hubContext.Clients.User(user.UserId.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            message = message,
                            category = category,
                            timestamp = entity.CreatedDate
                        });
                }
                catch (Exception ex)
                {
                    // Log and continue to ensure other users in the role get the alert
                    Console.WriteLine($"Failed to notify User {user.UserId} (Role: {targetRole}): {ex.Message}");
                }
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var entity = await _notifRepo.GetByIdAsync(notificationId);
            if (entity == null)
                throw new NotFoundException($"Notification {notificationId} not found.");

            entity.Status = "Read";
            await _notifRepo.UpdateAsync(entity);
        }
    }
}