using SecureOps.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class ThreatCampaignReadDto : ThreatCampaignCreateDto
    {
        public int CampaignId { get; set; }
    }
}
