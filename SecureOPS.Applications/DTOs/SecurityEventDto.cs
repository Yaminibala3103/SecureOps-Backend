using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
    public class SecurityEventDto
    {
        public int EventID { get; set; }

        [Required(ErrorMessage = "Source is required (e.g., Firewall-01, DB-Server).")]
        [StringLength(100, ErrorMessage = "Source name cannot exceed 100 characters.")]
        public string Source { get; set; }

        [Required(ErrorMessage = "EventType is required (e.g., SQL_Injection, Brute_Force).")]
        [StringLength(50, ErrorMessage = "EventType cannot exceed 50 characters.")]
        public string EventType { get; set; }

        [Required(ErrorMessage = "Severity level is required.")]
        [RegularExpression("^(Low|Medium|High|Critical)$",
            ErrorMessage = "Severity must be 'Low', 'Medium', 'High', or 'Critical'.")]
        public string Severity { get; set; }

        [Required]
        public DateTime DetectedTime { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Initial status is required.")]
        [StringLength(20, ErrorMessage = "Status is too long.")]
        public string Status { get; set; } = "New";
    }
}
