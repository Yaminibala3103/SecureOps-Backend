using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.Exceptions;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Domain.Entities;

namespace SecureOps.Application.Services
{
	public class AuditService : IAuditLogService
	{
		private readonly IAuditLogRepository _auditRepo;
		private readonly IMapper _mapper;

		public AuditService(IAuditLogRepository auditRepo, IMapper mapper)
		{
			_auditRepo = auditRepo;
			_mapper = mapper;
		}

		public async Task LogActionAsync(int userId, string action)
		{
			var auditEntry = new AuditLog
			{
				UserId = userId,
				Action = action,
				Timestamp = DateTime.Now,        // Matches Timestamp column

			};

			await _auditRepo.LogAction(auditEntry);
		}

		// Implementation of GetAllLogsAsync
		public async Task<IEnumerable<AuditLogDto>> GetAllLogsAsync()
		{
			var log = await _auditRepo.GetAll();
			return _mapper.Map<IEnumerable<AuditLogDto>>(log);
		}

		// Implementation of GetLogsByUserAsync
		public async Task<IEnumerable<AuditLogDto>> GetLogsByUserAsync(string userId)
		{
			int.TryParse(userId, out int parsedUserId);
			var allLogs = await _auditRepo.GetAll();

			// Filter logs by the specific User ID
			var filteredLogs = allLogs.Where(l => l.UserId == parsedUserId);
			return _mapper.Map<IEnumerable<AuditLogDto>>(filteredLogs);

		}
	}
}