using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class UpdateRemediationStatusDto
    {
        public RemediationStatus Status { get; set; }


    }
    public class UpdateRemediationOwnerDto
    {
        public required int AssignedToUserId { get; set; }
    }
}
