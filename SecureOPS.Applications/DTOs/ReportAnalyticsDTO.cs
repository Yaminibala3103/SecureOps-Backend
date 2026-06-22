using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel;

namespace SecureOPS.Applications.DTOs
{
	public class ReportAnalyticsDTO
	{
		[Range(0, int.MaxValue, ErrorMessage = "Total reports count cannot be negative.")]
		[DefaultValue(0)]
		public int TotalReports { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Open reports count must be a Positive  Number.")]
		[DefaultValue(0)]
		public int OpenReports { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Closed reports count must be a Positive  Number.")]
		[DefaultValue(0)]
		public int ClosedReports { get; set; }

		[Range(0, 100, ErrorMessage = "Risk Score must be between 0 and 100.")]
		[DefaultValue(0)]
		public int TotalIncidentCount { get; set; } // Sum of all incidents

		[Range(0, int.MaxValue, ErrorMessage = "Incident Count must be a Positive Number.")]
		[DefaultValue(0)]
		public double AverageRiskScore { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "High severity count must be a Positive  Number.")]
		[DefaultValue(0)]
		public int HighSeverityReports { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Average MTTD must be a Positive  Number.")]
		[DefaultValue(0)]
		public double AvgMTTD { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Average MTTR must be a Positive  Number.")]
		[DefaultValue(0)]
		public double AvgMTTR { get; set; }
	}

}
