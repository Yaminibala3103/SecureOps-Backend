using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Data;

namespace SecureOPS.Infrastructure.Repositories
{
	public class AuditLogRepository : IAuditLogRepository
	{
		private readonly SecureOpsDbContext _context;
		public AuditLogRepository(SecureOpsDbContext context) => _context = context;

		// Fulfills Requirement 4.1: The "Hands" that write to SQL Server
		public async Task LogAction(AuditLog log)
		{
			await _context.AuditLogs.AddAsync(log);
			await _context.SaveChangesAsync();
		}

		// Fulfills Requirement 2.7: Pulls raw data for Audit-ready reports
		public async Task<IEnumerable<AuditLog>> GetAll()
		{
			return await _context.AuditLogs.ToListAsync();
		}

		public async Task<IEnumerable<AuditLog>> GetByUserId(string userId)
		{
			return await _context.AuditLogs
				.Where(x => x.UserId == int.Parse(userId)) // Matches your int? UserId logic
				.ToListAsync();
		}
	}
}
