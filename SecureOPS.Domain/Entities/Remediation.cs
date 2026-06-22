using SecureOps.Domain;
using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SecureOPS.Domain.Entities;

public partial class Remediation : ISoftDeletable
{
    public int RemediationId { get; set; }

    public int? VulnerabilityId { get; set; }

    public int? AssetId { get; set; }

    public int? AssignedToUserId { get; set; }

    public RemediationStatus Status { get; set; }
    
    public DateTime? TargetDate { get; set; }
   
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
  
    public virtual User? AssignedTo { get; set; } //
   public virtual Asset? Asset { get; set; }
    public virtual Vulnerability? Vulnerability { get; set; }
}
