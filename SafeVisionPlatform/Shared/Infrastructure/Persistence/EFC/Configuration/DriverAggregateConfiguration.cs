using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeVisionPlatform.Driver.Domain.Model.Aggregates;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para el agregado DriverAggregate.
/// </summary>
public class DriverAggregateConfiguration : IEntityTypeConfiguration<DriverAggregate>
{
    public void Configure(EntityTypeBuilder<DriverAggregate> builder)
    {
        builder.ToTable("drivers");
        
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(d => d.RegisteredDate)
            .HasColumnName("registered_date")
            .IsRequired();

        builder.Property(d => d.UpdatedDate)
            .HasColumnName("updated_date")
            .IsRequired();

        // Configurar Value Object - DriverStatus
        builder.Property(d => d.Status)
            .HasConversion(
                v => (int)v.Value,
                v => DriverStatus.FromValue(v)
            )
            .HasColumnName("status")
            .IsRequired();

        // Configurar Value Object - DriverCredentials (mapeo a la misma tabla)
        builder.OwnsOne(d => d.Credentials, c =>
        {
            c.WithOwner().HasForeignKey("Id");
            c.Property<int>("Id");
            c.HasKey("Id");
            c.Property(cr => cr.Username)
                .HasColumnName("username")
                .IsRequired();
            c.Property(cr => cr.EncryptedPassword)
                .HasColumnName("encrypted_password")
                .IsRequired();
        });

        // Configurar Value Object - ContactInformation (mapeo a la misma tabla)
        builder.OwnsOne(d => d.ContactInformation, ci =>
        {
            ci.WithOwner().HasForeignKey("Id");
            ci.Property<int>("Id");
            ci.HasKey("Id");
            ci.Property(c => c.Phone)
                .HasColumnName("phone")
                .IsRequired();
            ci.Property(c => c.Email)
                .HasColumnName("contact_email")
                .IsRequired();
            ci.Property(c => c.Address)
                .HasColumnName("contact_address")
                .IsRequired();
        });

        // Relación con DriverProfile (uno a uno)
        builder.HasOne(d => d.Profile)
            .WithOne()
            .HasForeignKey<DriverProfile>(p => p.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con DriverLicense (uno a uno)
        builder.HasOne(d => d.License)
            .WithOne()
            .HasForeignKey<DriverLicense>(l => l.DriverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}