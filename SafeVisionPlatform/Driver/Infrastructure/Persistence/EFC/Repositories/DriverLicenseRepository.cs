using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace SafeVisionPlatform.Driver.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de DriverLicense usando Entity Framework Core.
/// </summary>
public class DriverLicenseRepository : IDriverLicenseRepository
{
    private readonly AppDbContext _context;

    public DriverLicenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DriverLicense?> FindByIdAsync(int id)
    {
        return await _context.DriverLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<DriverLicense>> ListAsync()
    {
        return await _context.DriverLicenses
            .AsNoTracking()
            .ToListAsync();
    }

    public void Update(DriverLicense entity)
    {
        _context.DriverLicenses.Update(entity);
    }

    public void Remove(DriverLicense entity)
    {
        _context.DriverLicenses.Remove(entity);
    }

    public async Task<DriverLicense?> GetByIdAsync(int licenseId)
    {
        return await _context.DriverLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == licenseId);
    }

    public async Task<DriverLicense?> GetByDriverIdAsync(int driverId)
    {
        return await _context.DriverLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.DriverId == driverId);
    }

    public async Task AddAsync(DriverLicense license)
    {
        await _context.DriverLicenses.AddAsync(license);
    }

    public async Task UpdateAsync(DriverLicense license)
    {
        _context.DriverLicenses.Update(license);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int licenseId)
    {
        var license = await GetByIdAsync(licenseId);
        if (license is not null)
        {
            _context.DriverLicenses.Remove(license);
        }
    }
}

