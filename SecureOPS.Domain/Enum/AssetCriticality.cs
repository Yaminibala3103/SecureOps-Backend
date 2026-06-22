using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Domain.Enum
{
    public enum AssetCriticality
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3,         // Add this to match your DB string
        MissionCritical = 4   // Keep this if you want both
    }
}
