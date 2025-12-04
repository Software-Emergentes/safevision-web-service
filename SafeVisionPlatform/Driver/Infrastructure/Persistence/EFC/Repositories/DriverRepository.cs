using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Driver.Domain.Model.Aggregates;
using SafeVisionPlatform.Driver.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;  

namespace SafeVisionPlatform.Driver.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de Driver usando Entity Framework Core.
/// </summary>
public class DriverRepository : IDriverRepository
{
    private readonly AppDbContext _context;

    public DriverRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DriverAggregate?> FindByIdAsync(int id)
    {
        return await _context.Drivers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<DriverAggregate>> ListAsync()
    {
        return await _context.Drivers
            .AsNoTracking()
            .ToListAsync();
    }

    public void Update(DriverAggregate entity)
    {
        _context.Drivers.Update(entity);
    }

    public void Remove(DriverAggregate entity)
    {
        _context.Drivers.Remove(entity);
    }

    public async Task<DriverAggregate?> GetByIdAsync(int driverId)
    {
        return await _context.Drivers
            .Include(d => d.Profile)      
            .Include(d => d.License)     
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == driverId);
    }

    public async Task<IEnumerable<DriverAggregate>> GetAllAsync()
    {
        return await _context.Drivers
            .Include(d => d.Profile)     
            .Include(d => d.License)      
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DriverAggregate?> GetByUserIdAsync(int userId)
    {
        return await _context.Drivers
            .Include(d => d.Profile)      
            .Include(d => d.License)    
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<IEnumerable<DriverAggregate>> GetByStatusAsync(int statusValue)
    {
        return await _context.Drivers
            .Include(d => d.Profile)      
            .Include(d => d.License)      
            .AsNoTracking()
            .Where(d => (int)d.Status.Value == statusValue)
            .ToListAsync();
    }

    public async Task<bool> ExistsByUserIdAsync(int userId)
    {
        return await _context.Drivers
            .AnyAsync(d => d.UserId == userId);
    }

    public async Task AddAsync(DriverAggregate driver)
    {
        await _context.Drivers.AddAsync(driver);
    }

    public async Task UpdateAsync(DriverAggregate driver)
    {
        _context.Drivers.Update(driver);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int driverId)
    {
        var driver = await GetByIdAsync(driverId);
        if (driver is not null)
        {
            _context.Drivers.Remove(driver);
        }
    }
}

