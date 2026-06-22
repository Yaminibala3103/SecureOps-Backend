using System;
using System.Collections.Generic;
using System.Text;
using SecureOPS.Domain.Entities;
using SecureOPS.Applications.DTOs;

namespace SecureOPS.Applications.ServiceInterfaces
{
	public interface IAuditLogService
	{
		Task LogActionAsync(int userId, string action);


		Task<IEnumerable<AuditLogDto>> GetAllLogsAsync();

		// Retrieves logs for a specific user to track performance
		Task<IEnumerable<AuditLogDto>> GetLogsByUserAsync(string userId);
	}
}
