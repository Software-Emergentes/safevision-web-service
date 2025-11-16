using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Repositories;

namespace SafeVisionPlatform.Trip.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio para SecurityConfiguration.
/// </summary>
public class SecurityConfigurationRepository : BaseRepository<SecurityConfiguration>, ISecurityConfigurationRepository
{
    public SecurityConfigurationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<SecurityConfiguration?> FindByIdAsync(int id)
    {
        return await Context.Set<SecurityConfiguration>()
            .FirstOrDefaultAsync(sc => sc.Id == id);
    }

    public async Task<SecurityConfiguration?> GetDefaultConfigurationAsync()
    {
        return await Context.Set<SecurityConfiguration>()
            .FirstOrDefaultAsync(sc => sc.IsDefault && sc.IsActive);
    }

    public async Task<SecurityConfiguration?> GetActiveConfigurationByManagerIdAsync(int managerId)
    {
        return await Context.Set<SecurityConfiguration>()
            .Where(sc => sc.ManagerId == managerId && sc.IsActive)
            .OrderByDescending(sc => sc.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<SecurityConfiguration?> GetActiveConfigurationByFleetIdAsync(int fleetId)
    {
        return await Context.Set<SecurityConfiguration>()
            .Where(sc => sc.FleetId == fleetId && sc.IsActive)
            .OrderByDescending(sc => sc.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SecurityConfiguration>> GetAllByManagerIdAsync(int managerId)
    {
        return await Context.Set<SecurityConfiguration>()
            .Where(sc => sc.ManagerId == managerId)
            .OrderByDescending(sc => sc.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<SecurityConfiguration>> GetAllActiveConfigurationsAsync()
    {
        return await Context.Set<SecurityConfiguration>()
            .Where(sc => sc.IsActive)
            .OrderByDescending(sc => sc.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<SecurityConfiguration>> GetAllConfigurationsAsync()
    {
        return await Context.Set<SecurityConfiguration>()
            .OrderByDescending(sc => sc.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(SecurityConfiguration configuration)
    {
        await Context.Set<SecurityConfiguration>().AddAsync(configuration);
    }

    public async Task UpdateAsync(SecurityConfiguration configuration)
    {
        Context.Set<SecurityConfiguration>().Update(configuration);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(SecurityConfiguration configuration)
    {
        Context.Set<SecurityConfiguration>().Remove(configuration);
        await Task.CompletedTask;
    }
}
