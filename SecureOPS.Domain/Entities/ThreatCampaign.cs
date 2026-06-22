using SecureOps.Domain;
using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureOPS.Domain.Entities;

public partial class ThreatCampaign : ISoftDeletable
{
    public int CampaignId { get; set; }
   
    public string? Name { get; set; }

    public string? Description { get; set; }
  
    public CampaignStatus Status { get; set; }
    public int? AssignedToUserId { get; set; }
    public virtual required User AssignedTo { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ThreatIndicator> ThreatIndicators { get; set; } = new List<ThreatIndicator>();
}
