using SecureOps.Domain;
using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SecureOPS.Domain.Entities;

public partial class ThreatIndicator : ISoftDeletable
{
    public int IndicatorId { get; set; }

    public int? CampaignId { get; set; }

    public IndicatorType Type { get; set; }

    public string? Value { get; set; }
    
    public int? Confidence { get; set; }

    public string? Source { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
  //  [ForeignKey("CampaignId")]
    public virtual ThreatCampaign? Campaign { get; set; }
}
