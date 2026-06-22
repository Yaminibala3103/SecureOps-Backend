using System;
using System.Collections.Generic;

namespace SecureOPS.Domain.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public string? Message { get; set; }

    public string? Category { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User? User { get; set; }
}
