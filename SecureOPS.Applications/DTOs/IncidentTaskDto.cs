using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
    public class IncidentTaskDto
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "IncidentId is required to associate this task with a case.")]
        [Range(1, int.MaxValue, ErrorMessage = "IncidentId must be a positive number.")]
        public int IncidentId { get; set; }

        [Required(ErrorMessage = "A task must be assigned to a UserID.")]
        [Range(1, int.MaxValue, ErrorMessage = "AssignedTo must be a valid UserID.")]
        public int AssignedTo { get; set; } // This is your UserID (e.g., 2472047)

        [Required(ErrorMessage = "Task description cannot be empty.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Description should be between 5 and 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Task status is required.")]
        [RegularExpression("^(Pending|In-Progress|Completed)$",
            ErrorMessage = "Status must be: Pending, In-Progress, or Completed.")]
        public required string Status { get; set; }
    }
}
