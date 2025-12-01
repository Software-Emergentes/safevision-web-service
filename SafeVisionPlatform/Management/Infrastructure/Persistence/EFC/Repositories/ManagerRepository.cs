using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Management.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.Management.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de gerentes.
/// </summary>
public class ManagerRepositoryImpl : BaseRepository<Manager>, IManagerRepository
{
    public ManagerRepositoryImpl(AppDbContext context) : base(context)
    {
    }

    public async Task<Manager?> GetByUserIdAsync(int userId)
    {
        return await Context.Set<Manager>()
            .FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<IEnumerable<Manager>> GetByRoleAsync(ManagerRole role)
    {
        return await Context.Set<Manager>()
            .Where(m => m.Role == role && m.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Manager>> GetActiveManagersAsync()
    {
        return await Context.Set<Manager>()
            .Where(m => m.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Manager>> GetManagersByFleetIdAsync(int fleetId)
    {
        return await Context.Set<Manager>()
            .Where(m => m.ManagedFleetIds.Contains(fleetId) && m.IsActive)
            .ToListAsync();
    }

    public async Task UpdateAsync(Manager manager)
    {
        Context.Set<Manager>().Update(manager);
        await Task.CompletedTask;
    }
}
