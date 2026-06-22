using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.RepositoryInterfaces
{
    public interface IIncidentTaskRepository
    {
        Task<IncidentTask> GetByIdAsync(int id);
        Task<IEnumerable<IncidentTask>> GetByIncidentIdAsync(int incidentId);
        Task<IncidentTask> AddAsync(IncidentTask task);
        Task UpdateAsync(IncidentTask task);
    }
}
