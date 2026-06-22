using System;
using System.Collections.Generic;
using System.Text;
using SecureOPS.Domain.Entities;
using SecureOPS.Applications.RepositoryInterfaces;

namespace SecureOPS.Applications.RepositoryInterfaces
{
	public interface IReportRepository
	{

		Task<SecurityReport?> GetByIdAsync(int id);
		Task<IEnumerable<SecurityReport>> GetAllAsync();
		Task AddAsync(SecurityReport report);
		Task UpdateAsync(SecurityReport report);
		Task DeleteAsync(int id);
		Task SoftDeleteAsync(int id);

	}
}
