using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.Driver.Domain.Model.Entities;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para la entidad DriverProfile.
/// </summary>
public class DriverProfileConfiguration : IEntityTypeConfiguration<DriverProfile>
{
    public void Configure(EntityTypeBuilder<DriverProfile> builder)
    {
        builder.ToTable("driver_profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.DriverId)
            .HasColumnName("driver_id")
            .IsRequired();

        builder.Property(p => p.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.PhotoUrl)
            .HasColumnName("photo_url")
            .HasMaxLength(500);

        builder.Property(p => p.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Address)
            .HasColumnName("address")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.YearsExperience)
            .HasColumnName("years_experience")
            .IsRequired();

        builder.Property(p => p.AcceptsPromotions)
            .HasColumnName("accepts_promotions")
            .IsRequired();

        builder.Property(p => p.ReceiveNotifications)
            .HasColumnName("receive_notifications")
            .IsRequired();

        builder.Property(p => p.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(p => p.UpdatedDate)
            .HasColumnName("updated_date")
            .IsRequired();
    }
}
