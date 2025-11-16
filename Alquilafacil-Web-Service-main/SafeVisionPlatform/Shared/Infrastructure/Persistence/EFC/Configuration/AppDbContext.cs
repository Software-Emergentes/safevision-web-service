using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.Entities;

namespace SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        // Enable Audit Fields Interceptors
        builder.AddCreatedUpdatedInterceptor();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Driver Context Configuration - Todas las configuraciones desde Shared
        builder.ApplyConfiguration(new DriverAggregateConfiguration());
        builder.ApplyConfiguration(new DriverProfileConfiguration());
        builder.ApplyConfiguration(new DriverLicenseConfiguration());

        // IAM Context Configuration
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Username).IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<User>().Property(u => u.Email).IsRequired();

        builder.Entity<UserRole>().HasKey(ur => ur.Id);
        builder.Entity<UserRole>().Property(ur => ur.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<UserRole>().Property(ur => ur.Role).IsRequired();
        builder.Entity<UserRole>().HasMany<User>().WithOne().HasForeignKey(u => u.RoleId);

        builder.Entity<MfaCode>().HasKey(m => m.Id);
        builder.Entity<MfaCode>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<MfaCode>().Property(m => m.Code).IsRequired().HasMaxLength(16);
        builder.Entity<MfaCode>().Property(m => m.CreatedAt).IsRequired();
        builder.Entity<MfaCode>().HasOne(m => m.User)
            .WithMany(u => u.MfaCodes)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Trip Context Configuration - Todas las configuraciones desde Shared
        builder.ApplyConfiguration(new TripAggregateConfiguration());
        builder.ApplyConfiguration(new AlertConfiguration());
        builder.ApplyConfiguration(new ReportConfiguration());
        builder.ApplyConfiguration(new CriticalNotificationConfiguration());
        builder.ApplyConfiguration(new SecurityConfigurationEntityConfiguration());

        // Apply SnakeCase Naming Convention
        builder.UseSnakeCaseWithPluralizedTableNamingConvention();
    }
}
