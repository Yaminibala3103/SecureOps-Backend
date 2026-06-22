using System;
using System.Collections.Generic;
using System.Text;

using SecureOPS.Domain.Entities;

namespace SecureOPS.Applications.RepositoryInterfaces
{
	public interface IAuditLogRepository
	{

		Task LogAction(AuditLog log);
		Task<IEnumerable<AuditLog>> GetAll(); // Added this to support GetAllLogs
		Task<IEnumerable<AuditLog>> GetByUserId(string userId);
	}
}
