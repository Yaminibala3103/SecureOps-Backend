using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Domain.Enum
{
    public enum RemediationStatus
    {
        Identified = 1,
        InProgress = 2,
        Resolved = 3,
        RiskAccepted = 4,
        FalsePositive = 5
    }
}
