using Microsoft.EntityFrameworkCore;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SecureOpsDbContext _context;

        public NotificationRepository(SecureOpsDbContext context)
        {
            _context = context;
        }

        // Get a single notification by its ID
        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _context.Notifications
                // Change n.UserId to the navigation property name (likely 'User')
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NotificationId == id);
        }
        

        // Get all notifications (useful for Admin/Audit logs)
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // Get notifications for a specific analyst (Module 2.8 specific)
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // Add a new notification (Triggered by the Alert/Watcher logic)
        public async Task<Notification> AddAsync(Notification entity)
        {
            await _context.Notifications.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // Update notification (e.g., changing status from 'Unread' to 'Read')
        public async Task UpdateAsync(Notification entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Remove a notification (e.g., clearing old logs)
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Notifications.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
