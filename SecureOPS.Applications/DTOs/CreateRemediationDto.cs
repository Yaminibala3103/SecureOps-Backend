using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class CreateRemediationDto
    {
        public int VulnerabilityId { get; set; }
        public int AssetId { get; set; }
        public int? AssignedToUserId { get; set; }
        public RemediationStatus Status { get; set; }
    }
}
