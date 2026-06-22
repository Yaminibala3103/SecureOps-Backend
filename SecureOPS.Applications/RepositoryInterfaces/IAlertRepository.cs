using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.RepositoryInterfaces
{
    public interface IAlertRepository
    {
        Task<Alert> GetByIdAsync(int id);
        Task<IEnumerable<Alert>> GetAllAsync();
        Task<Alert> AddAsync(Alert entity);
        Task UpdateAsync(Alert entity);
        Task DeleteAsync(int id);
    }
}
