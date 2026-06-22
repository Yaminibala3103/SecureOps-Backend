using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class ThreatCampaignCreateDto
    {
        [Required(ErrorMessage = "Campaign Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]

        public CampaignStatus Status { get; set; }
    }
}
