using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.Management.Domain.Model.Entities;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad Manager.
/// </summary>
public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.ToTable("Managers");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        builder.Property(m => m.UserId).IsRequired();
        builder.Property(m => m.FullName).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Email).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Phone).HasMaxLength(20).IsRequired(false);

        builder.Property(m => m.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.IsActive).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.UpdatedAt).IsRequired(false);

        // Configurar ManagedFleetIds como JSON
        builder.Property(m => m.ManagedFleetIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
            .HasColumnName("managed_fleet_ids");

        // Propiedades de persistencia directas para NotificationPreferences
        builder.Property(m => m.NotifyReceiveCriticalAlerts).IsRequired();
        builder.Property(m => m.NotifyReceiveDailyReports).IsRequired();
        builder.Property(m => m.NotifyReceiveWeeklyReports).IsRequired();
        builder.Property(m => m.NotifyEmailEnabled).IsRequired();
        builder.Property(m => m.NotifyPushEnabled).IsRequired();
        builder.Property(m => m.NotifySmsEnabled).IsRequired();

        // Ignorar el objeto NotificationPreferences para persistencia
        builder.Ignore(m => m.NotificationPreferences);

        // Índices
        builder.HasIndex(m => m.UserId).IsUnique();
        builder.HasIndex(m => m.Email);
        builder.HasIndex(m => m.Role);
        builder.HasIndex(m => m.IsActive);
    }
}

/// <summary>
/// Configuración de Entity Framework para la entidad Report (Management).
/// </summary>
public class ManagementReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("ManagementReports");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.ReportType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(1000).IsRequired(false);

        builder.Property(r => r.GeneratedById).IsRequired();
        builder.Property(r => r.DriverId).IsRequired(false);
        builder.Property(r => r.FleetId).IsRequired(false);

        builder.Property(r => r.StartDate).IsRequired();
        builder.Property(r => r.EndDate).IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.ExportFormat)
            .HasConversion<int?>()
            .IsRequired(false);

        builder.Property(r => r.ExportUrl).HasMaxLength(500).IsRequired(false);
        builder.Property(r => r.GeneratedAt).IsRequired();
        builder.Property(r => r.ExportedAt).IsRequired(false);

        // Ignorar objetos complejos para persistencia
        builder.Ignore(r => r.Metrics);
        builder.Ignore(r => r.RiskPatterns);

        // Índices
        builder.HasIndex(r => r.GeneratedById);
        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.FleetId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.GeneratedAt);
    }
}

/// <summary>
/// Configuración de Entity Framework para la entidad CriticalEvent.
/// </summary>
public class CriticalEventConfiguration : IEntityTypeConfiguration<CriticalEvent>
{
    public void Configure(EntityTypeBuilder<CriticalEvent> builder)
    {
        builder.ToTable("CriticalEvents");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.EventType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.DriverId).IsRequired();
        builder.Property(e => e.TripId).IsRequired(false);
        builder.Property(e => e.ManagedByManagerId).IsRequired(false);

        builder.Property(e => e.Description).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Severity).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Location).HasMaxLength(500).IsRequired(false);

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        // ActionsTaken se serializa como JSON
        builder.Property(e => e.ActionsTaken)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnName("actions_taken");

        builder.Property(e => e.InsuranceReference).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.EmergencyResponseDispatched).IsRequired();
        builder.Property(e => e.OccurredAt).IsRequired();
        builder.Property(e => e.ResolvedAt).IsRequired(false);
        builder.Property(e => e.Notes).HasMaxLength(2000).IsRequired(false);

        // Índices
        builder.HasIndex(e => e.DriverId);
        builder.HasIndex(e => e.TripId);
        builder.HasIndex(e => e.ManagedByManagerId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.OccurredAt);
        builder.HasIndex(e => e.EventType);
    }
}
