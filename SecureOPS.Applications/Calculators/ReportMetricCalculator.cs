using System;
using System.Collections.Generic;
using System.Text;
using SecureOPS.Domain.Entities;

namespace SecureOPS.Applications.Calculators
{
	public class ReportMetricsCalculator : IReportMetricsCalculator
	{
		public ReportMetricResult Calculate(List<Incident> incidents)
		{
			int incidentCount = incidents.Count;

			string status = incidents.Any(i => i.Status == "Open")
				? "Open"
				: incidents.Any(i => i.Status == "Medium")
					? "Pending"
					: "Closed";

			string severity = incidents.Any(i => i.Severity == "High")
				? "High"
					: "Low";

			double mttr = incidents
	     .Where(i => i.ClosedDate!=null && i.DetectedDate !=null)
	.Select(i => (i.ClosedDate - i.DetectedDate)!.Value.TotalHours)
	.DefaultIfEmpty(0)
	.Average();

			double mttd = incidents
				.Where(i => i.SlaDueDate!=null && i.DetectedDate!=null)
				.Select(i => (i.SlaDueDate - i.DetectedDate)!.Value.TotalHours)
				.DefaultIfEmpty(0)
				.Average();

			double riskScore = Math.Round(((mttd / 500) * 60) + ((mttr / 500) * 40));

			return new ReportMetricResult
			{
				IncidentCount = incidentCount,
				Mttd = mttd,
				Mttr = mttr,
				RiskScore = riskScore,
			
			};
		}
	}
}
