using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
    public class IncidentDto
    {
        public int IncidentId { get; set; }

        // AlertId is optional initially, but if present, must be valid
        [Range(1, int.MaxValue, ErrorMessage = "AlertId must be a positive integer.")]
        public int? AlertId { get; set; }

        [Required(ErrorMessage = "Incident title is mandatory.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 200 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Severity must be defined.")]
        [RegularExpression("^(Low|Medium|High|Critical)$",
            ErrorMessage = "Severity must be one of: Low, Medium, High, Critical")]
        public required string Severity { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DetectedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Current status is required.")]
        [RegularExpression("^(Open|Investigating|Contained|Closed)$",
            ErrorMessage = "Status must be: Open, Investigating, Contained, or Closed")]
        public required string Status { get; set; }

        // This collection represents the associated tasks managed in your Case Handling module
        public List<IncidentTaskDto> Tasks { get; set; } = new();
    }
}
