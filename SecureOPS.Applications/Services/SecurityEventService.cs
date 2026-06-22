using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Applications.Exceptions; // Use custom exceptions
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureOPS.Applications.Services
{
    public class SecurityEventService : ISecurityEventService
    {
        private readonly ISecurityEventRepository _eventRepo;
        private readonly IAlertService _alertService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public SecurityEventService(
            ISecurityEventRepository eventRepo,
            IAlertService alertService,
            INotificationService notificationService,
            IMapper mapper)
        {
            _eventRepo = eventRepo;
            _alertService = alertService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SecurityEventDto>> GetAllEventsAsync()
        {
            var entities = await _eventRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<SecurityEventDto>>(entities);
        }

        public async Task<SecurityEventDto> GetEventByIdAsync(int id)
        {
            var entity = await _eventRepo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($"Security Event with ID {id} not found.");

            return _mapper.Map<SecurityEventDto>(entity);
        }

        public async Task<SecurityEventDto> CreateEventAsync(SecurityEventDto eventDto)
        {
            if (eventDto == null)
                throw new AppValidationException("Security event data is required.");

            try
            {
                var entity = _mapper.Map<SecurityEvent>(eventDto);
                entity.DetectedTime = DateTime.Now;
                var result = await _eventRepo.AddAsync(entity);

                // THE WATCHER LOGIC: Auto-promote High/Critical events
                if (result.Severity == "High" || result.Severity == "Critical")
                {
                    await _alertService.CreateAlertAsync(new AlertDto
                    {
                        EventID = result.EventId,
                        RuleName = "Auto-Promotion: " + result.EventType,
                        Severity = result.Severity,
                        CreatedDate = DateTime.Now,
                        Status = "Open"
                    });

                    // ROLE-BASED NOTIFICATION LOGIC
                    try
                    {
                        if (result.Severity == "Critical")
                        {
                            await _notificationService.NotifyRoleAsync(
                                $"CRITICAL: {result.EventType} from {result.Source}", "Security", "L2 Analyst");

                            await _notificationService.NotifyRoleAsync(
                                $"IMMEDIATE ACTION REQUIRED: Incident {result.EventId}", "Incident", "Responder");
                        }
                        else if (result.Severity == "High")
                        {
                            await _notificationService.NotifyRoleAsync(
                                $"New High Severity Event: {result.EventType}", "Triage", "L1 Analyst");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Internal logging - we don't throw here so the event creation still completes
                        Console.WriteLine($"Notification Warning: {ex.Message}");
                    }
                }

                return _mapper.Map<SecurityEventDto>(result);
            }
            catch (Exception ex) when (!(ex is AppValidationException))
            {
                throw new Exception("An internal error occurred while processing the security event.", ex);
            }
        }

        public async Task UpdateEventAsync(int id, SecurityEventDto eventDto)
        {
            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Update failed: Security Event {id} does not exist.");

            try
            {
                _mapper.Map(eventDto, existing);
                await _eventRepo.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating security event {id}.", ex);
            }
        }

        public async Task DeleteEventAsync(int id)
        {
            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Delete failed: Security Event {id} not found.");

            await _eventRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<SecurityEventDto>> GetEventsBySeverityAsync(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity))
                throw new AppValidationException("Severity filter cannot be empty.");

            var all = await _eventRepo.GetAllAsync();
            var filtered = all.Where(e => e.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase));
            return _mapper.Map<IEnumerable<SecurityEventDto>>(filtered);
        }
    }
}