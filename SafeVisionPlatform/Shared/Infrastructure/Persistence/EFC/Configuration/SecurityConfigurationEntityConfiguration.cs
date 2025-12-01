using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad SecurityConfiguration.
/// </summary>
public class SecurityConfigurationEntityConfiguration : IEntityTypeConfiguration<SecurityConfiguration>
{
    public void Configure(EntityTypeBuilder<SecurityConfiguration> builder)
    {
        builder.ToTable("SecurityConfigurations");

        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id).ValueGeneratedOnAdd();

        // Propiedades de identificación
        builder.Property(sc => sc.ManagerId).IsRequired(false);
        builder.Property(sc => sc.FleetId).IsRequired(false);

        builder.Property(sc => sc.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(sc => sc.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        // Umbrales de Drowsiness
        builder.Property(sc => sc.DrowsinessEarThreshold)
            .HasPrecision(5, 3)
            .IsRequired();

        builder.Property(sc => sc.DrowsinessConsecutiveFrames).IsRequired();

        builder.Property(sc => sc.DrowsinessMinDurationSeconds)
            .HasPrecision(5, 2)
            .IsRequired();

        // Umbrales de MicroSleep
        builder.Property(sc => sc.MicroSleepEarThreshold)
            .HasPrecision(5, 3)
            .IsRequired();

        builder.Property(sc => sc.MicroSleepMinDurationSeconds)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(sc => sc.MicroSleepMaxOccurrencesPerWindow).IsRequired();
        builder.Property(sc => sc.MicroSleepWindowMinutes).IsRequired();

        // Umbrales de Distraction
        builder.Property(sc => sc.DistractionYawThresholdDegrees)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(sc => sc.DistractionPitchThresholdDegrees)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(sc => sc.DistractionMinDurationSeconds)
            .HasPrecision(5, 2)
            .IsRequired();

        // Umbrales de Alertas Críticas
        builder.Property(sc => sc.CriticalAlertsThreshold).IsRequired();
        builder.Property(sc => sc.CriticalAlertsWindowMinutes).IsRequired();
        builder.Property(sc => sc.NotificationCooldownMinutes).IsRequired();

        // Configuración de Safety Score
        builder.Property(sc => sc.SafetyScoreDrowsinessPenalty).IsRequired();
        builder.Property(sc => sc.SafetyScoreMicroSleepPenalty).IsRequired();
        builder.Property(sc => sc.SafetyScoreDistractionPenalty).IsRequired();
        builder.Property(sc => sc.SafetyScoreSafeTripBonus).IsRequired();

        // Metadatos
        builder.Property(sc => sc.IsActive).IsRequired();
        builder.Property(sc => sc.IsDefault).IsRequired();
        builder.Property(sc => sc.CreatedAt).IsRequired();
        builder.Property(sc => sc.UpdatedAt).IsRequired(false);
        builder.Property(sc => sc.CreatedBy).IsRequired(false);
        builder.Property(sc => sc.UpdatedBy).IsRequired(false);

        // Índices
        builder.HasIndex(sc => sc.ManagerId);
        builder.HasIndex(sc => sc.FleetId);
        builder.HasIndex(sc => sc.IsActive);
        builder.HasIndex(sc => sc.IsDefault);
        builder.HasIndex(sc => new { sc.ManagerId, sc.IsActive });
    }
}
