namespace SecureOPS.Domain.Entities;

public partial class User
{
	public int UserId { get; set; }
	public string? Name { get; set; }
	public string? Role { get; set; }
	public string? Email { get; set; }
	public string? Phone { get; set; }
	public string? PasswordHash { get; set; }

	public string? VerificationOtp { get; set; }
	public DateTime? OtpExpiry { get; set; }
	public bool IsEmailVerified { get; set; } = false;
	public string? RefreshToken { get; set; }
	public DateTime? RefreshTokenExpiry { get; set; }
	// --------------------------------------

	public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
	public virtual ICollection<IncidentTask> IncidentTasks { get; set; } = new List<IncidentTask>();
	public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
	public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
	public virtual ICollection<SecurityReport> SecurityReports { get; set; } = new List<SecurityReport>();
}