using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SecureOPS.Applications.DTOs
{
	public class UpdateReportDTO
	{
		[Required]
		//[JsonIgnore]
		public int ReportId { get; set; }

		[StringLength(100, MinimumLength = 5, ErrorMessage = "Scope must be between 5 and 100 characters.")]
		public string? Scope { get; set; }



	}
}
