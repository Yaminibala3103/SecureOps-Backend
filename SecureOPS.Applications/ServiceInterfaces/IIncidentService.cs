using SecureOPS.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
    public interface IIncidentService
    {
        Task<IncidentDto> PromoteAlertToIncidentAsync(int alertId, string title);
        Task<IncidentDto> GetIncidentDetailsAsync(int id);
        Task UpdateIncidentStatusAsync(int incidentId, string newStatus);
        Task<IncidentTaskDto> AssignTaskAsync(IncidentTaskDto taskDto);
    }
}
