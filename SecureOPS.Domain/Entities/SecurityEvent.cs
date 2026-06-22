using System;
using System.Collections.Generic;

namespace SecureOPS.Domain.Entities;

public partial class SecurityEvent
{
    public int EventId { get; set; }

    public string? Source { get; set; }

    public string? EventType { get; set; }

    public string? Severity { get; set; }

    public DateTime? DetectedTime { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
