using SecureOps.Domain;
using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace SecureOPS.Domain.Entities;


public partial class Asset : ISoftDeletable
{

    public int AssetId { get; set; }

    public string? AssetType { get; set; }

    public string? HostName { get; set; }

    public string? Ipaddress { get; set; }

    public string? Owner { get; set; }

    public AssetCriticality Criticality { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    [JsonIgnore]
	public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
	[JsonIgnore]
	public virtual ICollection<Remediation> Remediations { get; set; } = new List<Remediation>();
}
