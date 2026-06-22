using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Task<Notification> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification> AddAsync(Notification entity);
        Task UpdateAsync(Notification entity);
        Task DeleteAsync(int id);
    }
}
