using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class ThreatIndicatorCreateDto
    {
        public int? CampaignId { get; set; }
        [Required(ErrorMessage = ("Indicator type is Required(Ip,Domain,hash,URL"))]
        public IndicatorType Type { get; set; } // e.g., IP, Domain, Hash
        [Required(ErrorMessage = ("Indicator value is required"))]
        [StringLength(255, MinimumLength = 3)]
        public string? Value { get; set; }
        [Range(1, 100, ErrorMessage = ("Confidence range must between 1 and 100"))]
        public int? Confidence { get; set; } // 1-100
        public string? Source { get; set; }
    }

}
