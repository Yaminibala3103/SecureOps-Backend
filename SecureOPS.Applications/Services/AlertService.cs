using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Applications.Exceptions; // Use your custom exceptions
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureOPS.Applications.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepo;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public AlertService(IAlertRepository alertRepo, INotificationService notificationService, IMapper mapper)
        {
            _alertRepo = alertRepo;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AlertDto>> GetAllAlertsAsync()
        {
            var entities = await _alertRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<AlertDto>>(entities);
        }

        public async Task<AlertDto> GetAlertByIdAsync(int id)
        {
            var entity = await _alertRepo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($"Alert with ID {id} was not found.");

            return _mapper.Map<AlertDto>(entity);
        }

        public async Task<AlertDto> CreateAlertAsync(AlertDto alertDto)
        {
            if (alertDto == null)
                throw new AppValidationException("Alert data cannot be null.");

            try
            {
                var entity = _mapper.Map<Alert>(alertDto);
                var result = await _alertRepo.AddAsync(entity);

                // NOTIFICATION LOGIC
                await _notificationService.CreateSystemNotificationAsync(
                    $"ALERT: {result.RuleName} detected with {result.Severity} severity.",
                    "Alert"
                );

                return _mapper.Map<AlertDto>(result);
            }
            catch (Exception ex)
            {
                // Wrap database or mapping errors for the Global Handler
                throw new Exception("An error occurred while creating the security alert.", ex);
            }
        }

        public async Task UpdateAlertAsync(int id, AlertDto alertDto)
        {
            var existing = await _alertRepo.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Cannot update. Alert {id} does not exist.");

            try
            {
                _mapper.Map(alertDto, existing);
                await _alertRepo.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update alert {id}.", ex);
            }
        }

        public async Task DeleteAlertAsync(int id)
        {
            var existing = await _alertRepo.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Cannot delete. Alert {id} was not found.");

            await _alertRepo.DeleteAsync(id);
        }

        public async Task LinkAlertToIncidentAsync(int alertId, int incidentId)
        {
            var alert = await _alertRepo.GetByIdAsync(alertId);

            if (alert == null)
                throw new NotFoundException($"Link failed: Alert {alertId} not found.");

            // Logic Check: Prevent re-linking an alert already assigned to a different incident
            if (alert.IncidentId.HasValue && alert.IncidentId != incidentId)
                throw new AppValidationException($"Alert {alertId} is already linked to Incident {alert.IncidentId}.");

            try
            {
                alert.IncidentId = incidentId;
                await _alertRepo.UpdateAsync(alert);
            }
            catch (Exception ex)
            {
                throw new Exception("System error during alert-incident linking.", ex);
            }
        }
    }
}