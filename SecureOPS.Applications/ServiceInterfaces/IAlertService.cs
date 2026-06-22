using SecureOPS.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
    public interface IAlertService
    {
        Task<IEnumerable<AlertDto>> GetAllAlertsAsync();
        Task<AlertDto> GetAlertByIdAsync(int id);
        Task<AlertDto> CreateAlertAsync(AlertDto alertDto);
        Task UpdateAlertAsync(int id, AlertDto alertDto);
        Task DeleteAlertAsync(int id);

        // Watcher specific: Link multiple alerts to a single Incident
        Task LinkAlertToIncidentAsync(int alertId, int incidentId);
    }
}
