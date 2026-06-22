using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SecureOPS.Domain.Entities;

namespace SecureOPS.Domain.Data;

public partial class SecureOpsDbContext : DbContext
{
    public SecureOpsDbContext()
    {
    }

    public SecureOpsDbContext(DbContextOptions<SecureOpsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    public virtual DbSet<IncidentTask> IncidentTasks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Remediation> Remediations { get; set; }

    public virtual DbSet<SecurityEvent> SecurityEvents { get; set; }

    public virtual DbSet<SecurityReport> SecurityReports { get; set; }

    public virtual DbSet<ThreatCampaign> ThreatCampaigns { get; set; }

    public virtual DbSet<ThreatIndicator> ThreatIndicators { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vulnerability> Vulnerabilities { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer("Server=LTIN690213\\SQLEXPRESS;Database=SecureOPS;Trusted_Connection=True;TrustServerCertificate=True");
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Asset>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<ThreatIndicator>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Remediation>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Vulnerability>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<ThreatCampaign>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__Alerts__EBB16AED3C1E0973");

            entity.HasIndex(e => e.IncidentId, "IX_Alerts_IncidentID");

            entity.Property(e => e.AlertId).HasColumnName("AlertID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.IncidentId).HasColumnName("IncidentID");
            entity.Property(e => e.RuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Event).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Alerts__EventID__5812160E");

            entity.HasOne(d => d.Incident).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Alerts__Incident__59063A47");
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.AssetId).HasName("PK__Assets__4349237221D7F14D");

            entity.Property(e => e.AssetId).HasColumnName("AssetID").UseIdentityColumn(1,1);
            entity.Property(e => e.AssetType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Criticality)
                .HasMaxLength(20)
                .HasConversion<String>()
                .IsUnicode(false);
            entity.Property(e => e.HostName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Owner)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23B822A8741A");

            entity.Property(e => e.AuditId).HasColumnName("AuditID");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AuditLogs__UserI__5CD6CB2B");
        });

        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.IncidentId).HasName("PK__Incident__3D805392DF959298");

            entity.HasIndex(e => new { e.Status, e.Severity }, "IX_Incidents_Status_Severity");

            entity.Property(e => e.IncidentId).HasColumnName("IncidentID");
            entity.Property(e => e.AssetId).HasColumnName("AssetID");
            entity.Property(e => e.ClosedDate).HasColumnType("datetime");
            entity.Property(e => e.DetectedDate).HasColumnType("datetime");
            entity.Property(e => e.ResolutionNotes).HasColumnType("text");
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SlaDueDate)
                .HasColumnType("datetime")
                .HasColumnName("SLA_DueDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Asset).WithMany(p => p.Incidents)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Incidents__Asset__5441852A");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Incidents)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Incidents__Assig__5535A963");
        });

        modelBuilder.Entity<IncidentTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Incident__7C6949D1DC13003D");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IncidentId).HasColumnName("IncidentID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.IncidentTasks)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__IncidentT__Assig__6383C8BA");

            entity.HasOne(d => d.Incident).WithMany(p => p.IncidentTasks)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__IncidentT__Incid__628FA481");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E320493A443");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Notificat__UserI__6B24EA82");
        });

        modelBuilder.Entity<Remediation>(entity =>
        {
            entity.HasKey(e => e.RemediationId).HasName("PK__Remediat__AC36A051B40AA3C4");

            entity.ToTable("Remediation");

            entity.Property(e => e.RemediationId).HasColumnName("RemediationID");
            entity.Property(e => e.AssetId).HasColumnName("AssetID");
            entity.Property(e => e.AssignedToUserId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasConversion<String>()
                .IsUnicode(false);
            entity.Property(e => e.VulnerabilityId).HasColumnName("VulnerabilityID");

            entity.HasOne(d => d.Asset).WithMany(p => p.Remediations)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Remediati__Asset__6754599E");

            entity.HasOne(d => d.Vulnerability).WithMany(p => p.Remediations)
                .HasForeignKey(d => d.VulnerabilityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Remediati__Vulne__66603565");
        });

        modelBuilder.Entity<SecurityEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Security__7944C8704FCAE228");

            entity.HasIndex(e => e.DetectedTime, "IX_SecurityEvents_Time");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.DetectedTime).HasColumnType("datetime");
            entity.Property(e => e.EventType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SecurityReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Security__D5BD48E58593CA7D");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetricMttd).HasColumnName("Metric_MTTD");
            entity.Property(e => e.MetricMttr).HasColumnName("Metric_MTTR");
            entity.Property(e => e.Scope)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.SecurityReports)
                .HasForeignKey(d => d.GeneratedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__SecurityR__Gener__6EF57B66");
        });

        modelBuilder.Entity<ThreatCampaign>(entity =>
        {
            entity.HasKey(e => e.CampaignId).HasName("PK__ThreatCa__3F5E8D79575422A3");

            entity.Property(e => e.CampaignId).HasColumnName("CampaignID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasConversion<String>()
                .IsUnicode(false);
            entity.HasOne(tc => tc.AssignedTo)       
                    .WithMany()                         
                    .HasForeignKey(tc => tc.AssignedToUserId) 
                    .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ThreatIndicator>(entity =>
        {
            entity.HasKey(e => e.IndicatorId).HasName("PK__ThreatIn__4CDF25421F35868E");

            entity.Property(e => e.IndicatorId).HasColumnName("IndicatorID");
            entity.Property(e => e.CampaignId).HasColumnName("CampaignID");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasConversion<String>()
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Campaign).WithMany(p => p.ThreatIndicators)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ThreatInd__Campa__5FB337D6");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC9BE38B8D");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vulnerability>(entity =>
        {
            entity.HasKey(e => e.VulnerabilityId).HasName("PK__Vulnerab__087EB1E86757665F");

            entity.Property(e => e.VulnerabilityId).HasColumnName("VulnerabilityID");
            entity.Property(e => e.CVE)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CVE");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .HasConversion<String>()
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
