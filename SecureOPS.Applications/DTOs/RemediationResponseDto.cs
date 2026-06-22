using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class RemediationResponseDto
    {
        public int RemediationId { get; set; }
        public string? CVE { get; set; }       // From Vulnerability
        public string? HostName { get; set; }  // From Asset
        public int? AssignedToUserId { get; set; }
        public RemediationStatus Status { get; set; }    // String representation
        public SeverityLevel Severity { get; set; }  // String representation
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? TargetDate { get; set; }
       
    }
}
