using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel;

namespace SecureOPS.Applications.DTOs
{
	public class ReportDTO
	{

		[Range(1, int.MaxValue, ErrorMessage = "Report ID must be a positive number.")]
		[DefaultValue(0)]
		public int ReportId { get; set; }

		[Required(ErrorMessage = "The Scope field is mandatory.")]
		[StringLength(100, MinimumLength = 3, ErrorMessage = "Scope must be between 3 and 100 characters.")]
		public string? Scope { get; set; }

		[Required(ErrorMessage = "MTTD (Mean Time to Detect) is required.")]
		[Range(0, 5000, ErrorMessage = "MTTD must be a Positive Number.")]
		[DefaultValue(0)]
		public double? MetricMttd { get; set; }

		[Required(ErrorMessage = "MTTR (Mean Time to Resolve) is required.")]
		[Range(0, 5000, ErrorMessage = "MTTR must be a Positive Number.")]
		[DefaultValue(0)]
		public double? MetricMttr { get; set; }

		[Range(0, 100, ErrorMessage = "Risk Score must be between 0 and 100.")]
		[DefaultValue(0)]
		public int RiskScore { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Incident Count must be a Positive Number.")]
		[DefaultValue(0)]

		public int IncidentCount { get; set; }


		[Required(ErrorMessage = "Status cannot be empty.")]
		public string Status { get; set; } = string.Empty;

		[Required(ErrorMessage = "Severity cannot be empty.")]
		public string Severity { get; set; } = string.Empty;

		[DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
		public DateTime? GeneratedDate { get; set; }

		[Required(ErrorMessage = "The User ID of the generator is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "A valid User ID (1, 2, or 3) is required.")]
		[DefaultValue(0)]
		public int? GeneratedBy { get; set; }
	}
}
