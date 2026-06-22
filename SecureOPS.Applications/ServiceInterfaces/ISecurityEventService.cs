using SecureOPS.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
    public interface ISecurityEventService
    {
        Task<IEnumerable<SecurityEventDto>> GetAllEventsAsync();
        Task<SecurityEventDto> GetEventByIdAsync(int id);
        Task<SecurityEventDto> CreateEventAsync(SecurityEventDto eventDto);
        Task UpdateEventAsync(int id, SecurityEventDto eventDto);
        Task DeleteEventAsync(int id);

        // Watcher specific: Find events by severity for the heatmap
        Task<IEnumerable<SecurityEventDto>> GetEventsBySeverityAsync(string severity);
    }
}
