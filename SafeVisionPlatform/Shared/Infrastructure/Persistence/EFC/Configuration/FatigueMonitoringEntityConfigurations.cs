using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad DrowsinessEvent.
/// </summary>
public class DrowsinessEventConfiguration : IEntityTypeConfiguration<DrowsinessEvent>
{
    public void Configure(EntityTypeBuilder<DrowsinessEvent> builder)
    {
        builder.ToTable("DrowsinessEvents");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.DriverId).IsRequired();
        builder.Property(e => e.TripId).IsRequired();
        builder.Property(e => e.MonitoringSessionId).IsRequired();

        builder.Property(e => e.EventType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.DetectedAt).IsRequired();
        builder.Property(e => e.Processed).IsRequired();
        builder.Property(e => e.Notes).HasMaxLength(500).IsRequired(false);

        // Propiedades para SensorData mapeadas directamente
        builder.Property(e => e.SensorBlinkRate).IsRequired();
        builder.Property(e => e.SensorEyeOpenness).IsRequired();
        builder.Property(e => e.SensorMouthOpenness).IsRequired();
        builder.Property(e => e.SensorHeadTilt).IsRequired();
        builder.Property(e => e.SensorEyeClosureDuration).IsRequired();
        builder.Property(e => e.SensorCapturedAt).IsRequired();

        // Propiedades para SeverityScore mapeadas directamente
        builder.Property(e => e.SeverityValue).IsRequired();
        builder.Property(e => e.SeverityLevelStr).HasMaxLength(20).IsRequired();

        // Ignorar los Value Objects para persistencia
        builder.Ignore(e => e.SensorData);
        builder.Ignore(e => e.Severity);

        // Índices
        builder.HasIndex(e => e.DriverId);
        builder.HasIndex(e => e.TripId);
        builder.HasIndex(e => e.MonitoringSessionId);
        builder.HasIndex(e => e.DetectedAt);
        builder.HasIndex(e => e.Processed);
    }
}

/// <summary>
/// Configuración de Entity Framework para la entidad CriticalAlert (Fatigue Monitoring).
/// </summary>
public class FatigueCriticalAlertConfiguration : IEntityTypeConfiguration<CriticalAlert>
{
    public void Configure(EntityTypeBuilder<CriticalAlert> builder)
    {
        builder.ToTable("FatigueCriticalAlerts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.DriverId).IsRequired();
        builder.Property(a => a.TripId).IsRequired();
        builder.Property(a => a.ManagerId).IsRequired(false);

        builder.Property(a => a.AlertType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.Message)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.NotificationChannel)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.DrowsinessEventsCount).IsRequired();
        builder.Property(a => a.GeneratedAt).IsRequired();
        builder.Property(a => a.SentAt).IsRequired(false);
        builder.Property(a => a.AcknowledgedAt).IsRequired(false);
        builder.Property(a => a.AcknowledgedBy).IsRequired(false);
        builder.Property(a => a.ActionTaken).HasMaxLength(500).IsRequired(false);

        // Propiedades de severidad mapeadas directamente
        builder.Property(a => a.SeverityValue).IsRequired();
        builder.Property(a => a.SeverityLevel).HasMaxLength(20).IsRequired();

        // Ignorar el Value Object Severity para persistencia
        builder.Ignore(a => a.Severity);

        // Índices
        builder.HasIndex(a => a.DriverId);
        builder.HasIndex(a => a.TripId);
        builder.HasIndex(a => a.ManagerId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.GeneratedAt);
    }
}
