using System;
using System.Collections.Generic;
using System.Text;
using SecureOPS.Domain.Entities;

namespace SecureOPS.Applications.Calculators
{
	public interface IReportMetricsCalculator
	{
		ReportMetricResult Calculate(List<Incident> incidents);
	}
}
