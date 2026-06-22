using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
    public class AlertDto
    {
        public int AlertID { get; set; }

        [Required(ErrorMessage = "EventID is required to link this alert to a source event.")]
        [Range(1, int.MaxValue, ErrorMessage = "EventID must be a positive number.")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "RuleName is required.")]
        [StringLength(100, ErrorMessage = "RuleName cannot exceed 100 characters.")]
        public required string RuleName { get; set; }

        [Required(ErrorMessage = "Severity level is required (e.g., Low, Medium, High, Critical).")]
        [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Severity must be 'Low', 'Medium', 'High', or 'Critical'.")]
        public required string Severity { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status is too long.")]
        public required string Status { get; set; }

        // Optional: Adding the IncidentId we discussed for your "Reverse Stamping"
        public int? IncidentId { get; set; }
    }
}
