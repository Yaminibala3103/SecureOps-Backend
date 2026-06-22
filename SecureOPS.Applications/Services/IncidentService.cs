using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Applications.Exceptions; // Required for custom exceptions
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureOPS.Applications.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _incidentRepo;
        private readonly IAlertRepository _alertRepo;
        private readonly IIncidentTaskRepository _taskRepo;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public IncidentService(
            IIncidentRepository incidentRepo,
            IAlertRepository alertRepo,
            IIncidentTaskRepository taskRepo,
            INotificationService notificationService,
            IMapper mapper)
        {
            _incidentRepo = incidentRepo;
            _alertRepo = alertRepo;
            _taskRepo = taskRepo;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        // --- PROMOTE ALERT TO INCIDENT ---
        public async Task<IncidentDto> PromoteAlertToIncidentAsync(int alertId, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new AppValidationException("An incident title is required for promotion.");

            // 1. Get the Alert
            var alert = await _alertRepo.GetByIdAsync(alertId);
            if (alert == null)
                throw new NotFoundException($"Alert {alertId} not found. Promotion failed.");

            // Logic Check: Don't promote if already linked
            if (alert.IncidentId.HasValue)
                throw new AppValidationException($"Alert {alertId} is already linked to Incident {alert.IncidentId}.");

            try
            {
                // 2. Create the Incident
                var incident = new Incident
                {
                    Title = title,
                    Severity = alert.Severity,
                    DetectedDate = DateTime.Now,
                    Status = "Open"
                };
                var createdIncident = await _incidentRepo.AddAsync(incident);

                // 3. Link Alert to Incident
                alert.IncidentId = createdIncident.IncidentId;
                alert.Status = "Promoted";
                await _alertRepo.UpdateAsync(alert);

                return _mapper.Map<IncidentDto>(createdIncident);
            }
            catch (Exception ex)
            {
                throw new Exception("An internal error occurred while promoting the alert.", ex);
            }
        }

        // --- UPDATE STATUS & NOTIFY ---
        public async Task UpdateIncidentStatusAsync(int incidentId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new AppValidationException("New status must be provided.");

            var incident = await _incidentRepo.GetByIdAsync(incidentId);
            if (incident == null)
                throw new NotFoundException($"Incident {incidentId} not found.");

            try
            {
                incident.Status = newStatus;
                await _incidentRepo.UpdateAsync(incident);

                // Notify Manager of major transitions (e.g., Closed)
                if (newStatus == "Closed" || newStatus == "Contained")
                {
                    await _notificationService.NotifyRoleAsync(
                        $"Incident #{incidentId} is now {newStatus}", "Management", "Manager");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update status for Incident {incidentId}.", ex);
            }
        }

        // --- ASSIGN TASK ---
        public async Task<IncidentTaskDto> AssignTaskAsync(IncidentTaskDto taskDto)
        {
            if (taskDto == null)
                throw new AppValidationException("Task data cannot be null.");

            try
            {
                var entity = _mapper.Map<IncidentTask>(taskDto);
                entity.Status = "Pending";
                var result = await _taskRepo.AddAsync(entity);

                // Notify the specific user assigned to the task
                await _notificationService.CreateSystemNotificationAsync(
                    $"New Task Assigned: {entity.Description}", "Task", entity.AssignedTo);

                return _mapper.Map<IncidentTaskDto>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to assign investigative task.", ex);
            }
        }

        public async Task<IncidentDto> GetIncidentDetailsAsync(int id)
        {
            var entity = await _incidentRepo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($"Incident {id} not found.");

            return _mapper.Map<IncidentDto>(entity);
        }
    }
}