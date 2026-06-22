using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureOPS.Domain.Entities;

public partial class IncidentTask
{
	[Key]
	public int TaskId { get; set; }

    public int? IncidentId { get; set; }

    public int? AssignedTo { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Incident? Incident { get; set; }
}
