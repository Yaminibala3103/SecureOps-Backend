using SecureOPS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOps.Applications.DTOs
{
    public class AssetCreateDto
    {
        [Required(ErrorMessage = "Asset Type is required")]
        public string? AssetType { get; set; }
        [Required(ErrorMessage = "Hostname is required")]
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9.-]+$", ErrorMessage = "Hostname contains invalid characters")]
		public string? HostName { get; set; }
        [Required(ErrorMessage = "IP Address is required")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = " Invalid Ip address")]
        public string? Ipaddress { get; set; }
        [Required(ErrorMessage = "Owner is required")]
        public string? Owner { get; set; }
        [Required(ErrorMessage = "Criticality is required")]
        public AssetCriticality Criticality { get; set; }

    }
}
