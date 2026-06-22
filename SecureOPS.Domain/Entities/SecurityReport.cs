using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SecureOps.Domain;

namespace SecureOPS.Domain.Entities;

public partial class SecurityReport:ISoftDeletable
{
    public int ReportId { get; set; }

    public string? Scope { get; set; }

    public double? MetricMttd { get; set; }

    public double? MetricMttr { get; set; }

    public int? IncidentCount { get; set; }

    public int? RiskScore { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public int? GeneratedBy { get; set; }
	public bool IsDeleted { get; set; } = false;
	public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
	public virtual User? GeneratedByNavigation { get; set; }
}
