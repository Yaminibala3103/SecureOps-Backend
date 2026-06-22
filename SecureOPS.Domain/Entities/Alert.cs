using System;
using System.Collections.Generic;

namespace SecureOPS.Domain.Entities;

public partial class Alert
{
    public int AlertId { get; set; }

    public int? EventId { get; set; }

    public int? IncidentId { get; set; }

    public string? RuleName { get; set; }

    public string? Severity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public virtual SecurityEvent? Event { get; set; }

    public virtual Incident? Incident { get; set; }
}
