using System;
using System.Collections.Generic;
using SecureOPS.Domain.Entities;
using SecureOPS.Applications.DTOs;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
	public interface IReportService
	{
		Task<IEnumerable<ReportResponseDTO>> GetAllReports();
		Task<ReportResponseDTO?> GetReportById(int id);
		Task<SecurityReport> CreateReport(CreateReportDTO report, int currUserId);
		Task UpdateReport(UpdateReportDTO report, int currUserId);
		Task DeleteReport(int id, int currUserId);
		//Task ChangeStatus(int id, string status); 
		Task<IEnumerable<ReportResponseDTO>> GetReportsByStatus(string status);

		//Task<IEnumerable<Incident>> GetByIdAsync(int reportId);
		Task<IEnumerable<ReportResponseDTO>> GetReportsBySeverity(string severity);

		Task<ReportAnalyticsDTO> GenerateStrategicAnalyticsAsync();


	}
}
