using SafeVisionPlatform.Trip.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework para el agregado TripAggregate.
/// </summary>
public class TripAggregateConfiguration : IEntityTypeConfiguration<TripAggregate>
{
    public void Configure(EntityTypeBuilder<TripAggregate> builder)
    {
        builder.ToTable("Trips");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.DriverId).IsRequired();
        builder.Property(t => t.VehicleId).IsRequired();

        // Configurar el Value Object TripStatus
        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();

        // Configurar el Value Object TripTime (mapeo a la misma tabla)
        builder.OwnsOne(t => t.Time, timeBuilder =>
        {
            timeBuilder.WithOwner().HasForeignKey("Id");
            timeBuilder.Property<int>("Id");
            timeBuilder.HasKey("Id");

            timeBuilder.Property(tt => tt.StartTime)
                .HasColumnName("StartTime")
                .IsRequired();

            timeBuilder.Property(tt => tt.EndTime)
                .HasColumnName("EndTime")
                .IsRequired(false);
        });

        // Configurar el Value Object TripDataPolicy (mapeo a la misma tabla)
        builder.OwnsOne(t => t.DataPolicy, policyBuilder =>
        {
            policyBuilder.WithOwner().HasForeignKey("Id");
            policyBuilder.Property<int>("Id");
            policyBuilder.HasKey("Id");

            policyBuilder.Property(p => p.SyncToCloud)
                .HasColumnName("SyncToCloud")
                .IsRequired();

            policyBuilder.Property(p => p.SyncIntervalMinutes)
                .HasColumnName("SyncIntervalMinutes")
                .IsRequired();

            policyBuilder.Property(p => p.IncludeAlerts)
                .HasColumnName("IncludeAlerts")
                .IsRequired();

            policyBuilder.Property(p => p.IncludeMetrics)
                .HasColumnName("IncludeMetrics")
                .IsRequired();
        });

        // Relación con Alerts
        builder.HasMany(t => t.Alerts)
            .WithOne()
            .HasForeignKey(a => a.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con Report (opcional)
        builder.HasOne(t => t.Report)
            .WithOne()
            .HasForeignKey<Report>(r => r.TripId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired(false);
    }
}

/// <summary>
/// Configuración de Entity Framework para la entidad Alert.
/// </summary>
public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.TripId).IsRequired();
        builder.Property(a => a.AlertType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Severity)
            .IsRequired(false);

        builder.Property(a => a.DetectedAt).IsRequired();
        builder.Property(a => a.Acknowledged).IsRequired();

        // Propiedades de feedback
        builder.Property(a => a.MarkedAsFalseAlarm).IsRequired();
        builder.Property(a => a.FeedbackComment)
            .HasMaxLength(1000)
            .IsRequired(false);
        builder.Property(a => a.FeedbackSubmittedAt).IsRequired(false);
        builder.Property(a => a.FeedbackSubmittedBy).IsRequired(false);

        // Índices
        builder.HasIndex(a => a.TripId);
        builder.HasIndex(a => a.AlertType);
        builder.HasIndex(a => a.DetectedAt);
        builder.HasIndex(a => a.MarkedAsFalseAlarm);
    }
}

/// <summary>
/// Configuración de Entity Framework para la entidad Report.
/// </summary>
public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.TripId).IsRequired(false);
        builder.Property(r => r.DriverId).IsRequired();
        builder.Property(r => r.VehicleId).IsRequired();

        builder.Property(r => r.StartTime).IsRequired();
        builder.Property(r => r.EndTime).IsRequired();
        builder.Property(r => r.DurationMinutes).IsRequired();
        builder.Property(r => r.DistanceKm).IsRequired();
        builder.Property(r => r.AlertCount).IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(r => r.Recipient)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.SentAt).IsRequired(false);

        // Índices
        builder.HasIndex(r => r.TripId).IsUnique();
        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.Status);
    }
}

/// <summary>
/// Configuración de Entity Framework para la entidad CriticalNotification.
/// </summary>
public class CriticalNotificationConfiguration : IEntityTypeConfiguration<CriticalNotification>
{
    public void Configure(EntityTypeBuilder<CriticalNotification> builder)
    {
        builder.ToTable("CriticalNotifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedOnAdd();

        builder.Property(n => n.DriverId).IsRequired();
        builder.Property(n => n.TripId).IsRequired();
        builder.Property(n => n.ManagerId).IsRequired(false);

        builder.Property(n => n.Severity)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.AlertType).IsRequired();
        builder.Property(n => n.CriticalAlertsCount).IsRequired();

        builder.Property(n => n.Message)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.Timestamp).IsRequired();

        builder.Property(n => n.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Channel)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.SentAt).IsRequired(false);
        builder.Property(n => n.ReadAt).IsRequired(false);
        builder.Property(n => n.AcknowledgedAt).IsRequired(false);

        // Índices
        builder.HasIndex(n => n.DriverId);
        builder.HasIndex(n => n.TripId);
        builder.HasIndex(n => n.ManagerId);
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.Timestamp);
    }
}
