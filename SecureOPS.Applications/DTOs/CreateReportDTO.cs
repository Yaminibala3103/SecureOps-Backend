using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
	public class CreateReportDTO
	{
		[Required(ErrorMessage = "The Scope field is required.")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Scope must be between 5 and 100 characters.")]
		public string? Scope { get; set; }

	




	}
}
