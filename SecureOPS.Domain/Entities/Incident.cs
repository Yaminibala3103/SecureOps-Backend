using System;
using System.Collections.Generic;

namespace SecureOPS.Domain.Entities;

public partial class Incident
{
    public int IncidentId { get; set; }

    public string? Title { get; set; }

    public string? Severity { get; set; }

    public DateTime? DetectedDate { get; set; }

    public string? Status { get; set; }

    public int? AssetId { get; set; }

    public int? AssignedTo { get; set; }

    public string? ResolutionNotes { get; set; }

    public DateTime? ClosedDate { get; set; }

    public DateTime? SlaDueDate { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual Asset? Asset { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual ICollection<IncidentTask> IncidentTasks { get; set; } = new List<IncidentTask>();
}
