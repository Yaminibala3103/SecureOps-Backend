using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.RepositoryInterfaces
{
    public interface IIncidentRepository
    {
        Task<Incident> GetByIdAsync(int id);
        Task<IEnumerable<Incident>> GetAllAsync();
        Task<Incident> AddAsync(Incident incident);
        Task UpdateAsync(Incident incident);
    }
}
