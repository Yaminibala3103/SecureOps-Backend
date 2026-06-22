using SecureOps.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class ThreatIndicatorReadDto : ThreatIndicatorCreateDto
    {
        public int IndicatorId { get; set; }
        public string? ThreatcampaignName { get; set; }
    }
}
