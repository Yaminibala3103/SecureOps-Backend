using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;

namespace SecureOPS.Infrastructure.Repositories
{
	public class ReportRepository : IReportRepository
	{
		private readonly SecureOpsDbContext _context;
		public ReportRepository(SecureOpsDbContext context)
		{
			_context = context;

		}
		public async Task<SecurityReport?> GetByIdAsync(int id)
		{
			return await _context.SecurityReports.FirstOrDefaultAsync(r => r.ReportId == id);
		}

		public async Task<IEnumerable<SecurityReport>> GetAllAsync()
		{
			// Filter out records where IsDeleted is true
			return await _context.SecurityReports
				.Where(r => !r.IsDeleted)
				.ToListAsync();
		}


		public async Task AddAsync(SecurityReport report)
		{
			report.ReportId = 0;
			await _context.SecurityReports.AddAsync(report);
			await _context.SaveChangesAsync(); // Saves MTTD/MTTR metrics
		}

		public async Task UpdateAsync(SecurityReport report)
		{
			// 1. Check if the ID exists in the database FIRST
			var existing = await _context.SecurityReports.FindAsync(report.ReportId);

			if (existing == null)
			{
				// This is where you handle the "ID Mismatch" or "Not Found"
				throw new KeyNotFoundException($"ID Mismatch: Report with ID {report.ReportId} was not found.");
			}

			// 2. If it exists, update the values
			_context.Entry(existing).CurrentValues.SetValues(report);

			// 3. Save the changes
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var report = await _context.SecurityReports.FindAsync(id);
			if (report != null)
			{
				_context.SecurityReports.Remove(report);
				await _context.SaveChangesAsync();
			}

		}

		public async Task SoftDeleteAsync(int id)
		{
			var report = await _context.SecurityReports.FindAsync(id);
			if (report != null)
			{
				report.IsDeleted = true;
				report.DeletedAt = DateTime.Now; // Records the exact moment of deletion
				await _context.SaveChangesAsync();
			}
		}
	}
}
