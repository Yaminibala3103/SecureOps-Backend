using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
	public class ReportResponseDTO
	{
		[Key]
		[Required(ErrorMessage = "Response must include a Report ID.")]
		[Range(1, int.MaxValue, ErrorMessage = "Report ID must be a positive integer.")]
		[DefaultValue(0)]
		public int ReportId { get; set; }

		[Required(ErrorMessage = "Scope cannot be null in the response.")]
		public string? Scope { get; set; }

		[Required(ErrorMessage = "MTTD value is missing from the response.")]
		[Range(0, double.MaxValue, ErrorMessage = "MTTD must be a non-negative value.")]
		[DefaultValue(0)]
		public double? MetricMttd { get; set; }

		[Required(ErrorMessage = "MTTR value is missing from the response.")]
		[Range(0, double.MaxValue, ErrorMessage = "MTTR must be a non-negative value.")]
		[DefaultValue(0)]
		public double? MetricMttr { get; set; }

		[Range(0, 100, ErrorMessage = "Risk Score must be between 0 and 100.")]
		[DefaultValue(0)]
		public int RiskScore { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Incident Count must be a Positive Number.")]
		[DefaultValue(0)]
		public int IncidentCount
		{
			get; set;
		}

		[Required(ErrorMessage = "Report Status must be calculated and provided.")]
		public string Status { get; set; } = string.Empty;

		[Required(ErrorMessage = "Report Severity must be calculated and provided.")]
		public string Severity { get; set; } = string.Empty;

		[Required(ErrorMessage = "Generation date is missing.")]
		public DateTime? GeneratedDate { get; set; }
	}
}
