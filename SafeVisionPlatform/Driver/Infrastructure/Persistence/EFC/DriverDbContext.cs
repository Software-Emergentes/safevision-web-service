using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Driver.Domain.Model.Aggregates;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Driver.Infrastructure.Persistence.EFC;

/// <summary>
/// Contexto de Entity Framework Core para el bounded context Driver.
/// </summary>
public class DriverDbContext : DbContext, IUnitOfWork
{
    public DbSet<DriverAggregate> Drivers { get; set; } = null!;
    public DbSet<DriverProfile> DriverProfiles { get; set; } = null!;
    public DbSet<DriverLicense> DriverLicenses { get; set; } = null!;

    public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones desde Shared
        modelBuilder.ApplyConfiguration(new DriverAggregateConfiguration());
        modelBuilder.ApplyConfiguration(new DriverProfileConfiguration());
        modelBuilder.ApplyConfiguration(new DriverLicenseConfiguration());
    }

    public async Task CompleteAsync()
    {
        await SaveChangesAsync();
    }
}
