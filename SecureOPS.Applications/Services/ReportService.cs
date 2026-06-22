using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain;
using SecureOPS.Applications.Exceptions;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.Calculators;

namespace SecureOps.Application.Services
{
	public class ReportService : IReportService
	{
		private readonly IReportRepository _reportRepository;
		private readonly IMapper _mapper;
		private readonly IAuditLogService _auditLogService;
		private readonly IIncidentRepository _incidentRepository;
		private readonly IReportMetricsCalculator _metricsCalculator;

		public ReportService(IReportRepository reportRepository, IMapper mapper, IAuditLogService auditLogService, IIncidentRepository incidentRepository, IReportMetricsCalculator  metricsCalculator)
		{
			_reportRepository = reportRepository;
			_mapper = mapper;
			_auditLogService = auditLogService;
			_incidentRepository = incidentRepository;

			_metricsCalculator = metricsCalculator;
		}
		public async Task<IEnumerable<ReportResponseDTO>> GetAllReports()
		{

			var entities = await _reportRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ReportResponseDTO>>(entities);
		}
		public async Task<ReportResponseDTO?> GetReportById(int id)
		{
			var entity = await _reportRepository.GetByIdAsync(id);
			if (entity == null || entity.IsDeleted)
				throw new NotFoundException($"Report with ID {id} not found.");

			return _mapper.Map<ReportResponseDTO>(entity);
		}

		public async Task<SecurityReport> CreateReport(CreateReportDTO reportDto, int currUserId)
		{
			//var entity = _mapper.Map<SecurityReport>(reportDto);

			//entity.GeneratedBy = currUserId;
			//entity.GeneratedDate = DateTime.UtcNow;
			//entity.IsDeleted = false;

			//await _reportRepository.AddAsync(entity);

			//await _auditLogService.LogActionAsync(currUserId, $"Created Report {entity.ReportId}");

			//return entity;
			var incidents = (await _incidentRepository.GetAllAsync()).ToList();

			if (!incidents.Any())
				throw new Exception("No incidents found to generate report.");

			var metrics = _metricsCalculator.Calculate(incidents);

			var entity = new SecurityReport
			{
				Scope = reportDto.Scope,
				IncidentCount = metrics.IncidentCount,
				MetricMttd = Math.Round(metrics.Mttd, 2),
				MetricMttr = Math.Round(metrics.Mttr, 2),
				RiskScore = (int?)metrics.RiskScore,

				GeneratedBy = currUserId,
				GeneratedDate = DateTime.UtcNow,
				IsDeleted = false
			};

			await _reportRepository.AddAsync(entity);

			await _auditLogService.LogActionAsync(
				currUserId,
				$"Security Report Generated  {entity.ReportId}");

			return entity;
		}

		public async Task UpdateReport(UpdateReportDTO reportDto, int currUserId)

		{

			var existingEntity = await _reportRepository.GetByIdAsync(reportDto.ReportId);



			if (existingEntity is null || existingEntity.IsDeleted)

				throw new Exception("Report not found.");
			var incidents = (await _incidentRepository.GetAllAsync()).ToList();

			if (incidents.Any())
			{
				var metrics = _metricsCalculator.Calculate(incidents);
				existingEntity.MetricMttd = Math.Round(metrics.Mttd, 2);
				existingEntity.MetricMttr = Math.Round(metrics.Mttr, 2);
				existingEntity.RiskScore = (int?)metrics.RiskScore;
				existingEntity.IncidentCount = metrics.IncidentCount;
				existingEntity.GeneratedBy = currUserId;
				existingEntity.GeneratedDate = DateTime.UtcNow;


			}

			_mapper.Map(reportDto, existingEntity);

			await _reportRepository.UpdateAsync(existingEntity);

			await _auditLogService.LogActionAsync(currUserId, $"Updated Report{existingEntity.ReportId}");



		}

		public async Task DeleteReport(int id, int currUserId)
		{
			var entity = await _reportRepository.GetByIdAsync(id);
			if (entity == null || entity.IsDeleted)
			{
				throw new Exception($"Delete failed: Report {id} not found.");
			}
			entity.IsDeleted = true;
			entity.DeletedAt = DateTime.UtcNow;
			await _reportRepository.UpdateAsync(entity);

			await _auditLogService.LogActionAsync(currUserId, $"Deleted Report{entity.ReportId}");
		}





public async Task<IEnumerable<ReportResponseDTO>> GetReportsByStatus(string status)
		{
			var reports = await _reportRepository.GetAllAsync();
			var allIncidents = (await _incidentRepository.GetAllAsync());
			var filteredReports = reports.Where(r => !r.IsDeleted).Where(r =>
			{



				// Calculate status dynamically
				string reportStatus = allIncidents.Any(i => i.Status == "Open")
					? "Open"
					: allIncidents.Any(i => i.Status == "Medium")
						? "Pending"
						: "Closed";

				return string.Equals(reportStatus, status, StringComparison.OrdinalIgnoreCase);
			}).ToList();
			

			return _mapper.Map<IEnumerable<ReportResponseDTO>>(filteredReports);
		}

		public async Task<IEnumerable<ReportResponseDTO>> GetReportsBySeverity(string severity)
		{
			var reports = await _reportRepository.GetAllAsync();
			var filteredReports = new List<SecurityReport>();

			foreach (var report in reports.Where(r => !r.IsDeleted))
			{
				// Calculate severity from RiskScore
				var reportSeverity = report.RiskScore >= 20
					? "Critical"
					: report.RiskScore >= 10
						? "High": report.RiskScore >= 5 ?"Medium"
						: "Low";

				if (string.Equals(reportSeverity, severity, StringComparison.OrdinalIgnoreCase))
					filteredReports.Add(report);
			}

			return _mapper.Map<IEnumerable<ReportResponseDTO>>(filteredReports);
		}

		public async Task<ReportAnalyticsDTO> GenerateStrategicAnalyticsAsync()
		{

			var reports = await _reportRepository.GetAllAsync();
			var activeReports = reports.Where(x => !x.IsDeleted).ToList();

			return new ReportAnalyticsDTO
			{
				TotalReports = activeReports.Count,
				TotalIncidentCount = activeReports.Sum(x => x.IncidentCount ?? 0),

				// Average danger level of the environment
				AverageRiskScore = activeReports.Any() ? Math.Round(activeReports.Average(x => x.RiskScore ?? 0), 2) : 0,

				OpenReports = activeReports.Count(x => x.MetricMttr != 1 && x.MetricMttr != 2),
				ClosedReports = activeReports.Count(x => x.MetricMttr == 1),
				HighSeverityReports = activeReports.Count(x => x.MetricMttd > 10),
				AvgMTTD = activeReports.Any()
			? Math.Round(activeReports.Average(x => x.MetricMttd)??0, 2)
			: 0,

				AvgMTTR = activeReports.Any()
			? Math.Round(activeReports.Average(x => x.MetricMttr)??0, 2)
			: 0
			};
		}

	}
}