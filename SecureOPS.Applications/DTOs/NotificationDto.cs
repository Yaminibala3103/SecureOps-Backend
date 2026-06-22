using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
    public class NotificationDto
    {
        public int NotificationID { get; set; }

        // UserID is nullable because some notifications might be "Broadcasts" 
        // to a whole role rather than one individual.
        [Range(1, int.MaxValue, ErrorMessage = "If provided, UserID must be a positive number.")]
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Notification message is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Message must be between 5 and 500 characters.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Category is required (e.g., Alert, Incident, Task).")]
        [StringLength(50, ErrorMessage = "Category name is too long.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Unread|Read)$", ErrorMessage = "Status must be either 'Unread' or 'Read'.")]
        public string Status { get; set; } = "Unread";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
