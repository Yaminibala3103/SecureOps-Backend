using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.RepositoryInterfaces
{
    public interface ISecurityEventRepository
    {
        Task<SecurityEvent> GetByIdAsync(int id);
        Task<IEnumerable<SecurityEvent>> GetAllAsync();
        Task<SecurityEvent> AddAsync(SecurityEvent entity);
        Task UpdateAsync(SecurityEvent entity);
        Task DeleteAsync(int id);
    }
}
