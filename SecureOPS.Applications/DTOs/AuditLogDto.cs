using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
	public class AuditLogDto
	{

		[Key]
		public int AuditId { get; set; }

		[Required(ErrorMessage = "User ID is required for audit tracking.")]
		[Range(1, int.MaxValue, ErrorMessage = "A valid User ID must be provided.")]
		[DefaultValue(0)]
		public int? UserId { get; set; }

		[Required(ErrorMessage = "The action description cannot be empty.")]
		[StringLength(500, ErrorMessage = "Action description cannot exceed 500 characters.")]
		public string Action { get; set; } = string.Empty;

		[Required(ErrorMessage = "Timestamp is required.")]
		public DateTime Timestamp { get; set; }
	}
}
