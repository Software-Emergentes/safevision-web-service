using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para la entidad DriverLicense.
/// </summary>
public class DriverLicenseConfiguration : IEntityTypeConfiguration<DriverLicense>
{
    public void Configure(EntityTypeBuilder<DriverLicense> builder)
    {
        builder.ToTable("driver_licenses");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.DriverId)
            .HasColumnName("driver_id")
            .IsRequired();

        // Configurar Value Object - LicenseNumber
        builder.Property(l => l.LicenseNumber)
            .HasConversion(
                v => v.ToString(),
                v => LicenseNumber.Create(v)
            )
            .HasColumnName("license_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.Category)
            .HasColumnName("category")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(l => l.IssuedDate)
            .HasColumnName("issued_date")
            .IsRequired();

        builder.Property(l => l.ExpirationDate)
            .HasColumnName("expiration_date")
            .IsRequired();

        builder.Property(l => l.IsValidated)
            .HasColumnName("is_validated")
            .IsRequired();

        builder.Property(l => l.ValidatedDate)
            .HasColumnName("validated_date")
            .IsRequired();
    }
}
