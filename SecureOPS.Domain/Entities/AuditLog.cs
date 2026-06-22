using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SecureOPS.Domain.Entities;

public class AuditLog
{
	[Key]
	[Column("AuditID")]
	public int AuditId { get; set; }

	public int? UserId { get; set; }

	public string? Action { get; set; }

	public DateTime? Timestamp { get; set; }

	[ForeignKey("UserId")]
	[JsonIgnore]
	public virtual User? User { get; set; }
}
