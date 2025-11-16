using SafeVisionPlatform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Repositories;

namespace SafeVisionPlatform.Trip.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación concreta del TripRepository.
/// Gestiona las operaciones CRUD del viaje sobre la base de datos relacional.
/// </summary>
public class TripRepository : ITripRepository
{
    private readonly AppDbContext _context;

    public TripRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TripAggregate entity)
    {
        await _context.Set<TripAggregate>().AddAsync(entity);
    }

    public async Task<TripAggregate?> FindByIdAsync(int id)
    {
        return await _context.Set<TripAggregate>()
            .Include(t => t.Alerts)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TripAggregate>> ListAsync()
    {
        return await _context.Set<TripAggregate>()
            .Include(t => t.Alerts)
            .ToListAsync();
    }

    public void Update(TripAggregate entity)
    {
        _context.Set<TripAggregate>().Update(entity);
    }

    public void Remove(TripAggregate entity)
    {
        _context.Set<TripAggregate>().Remove(entity);
    }

    public async Task<IEnumerable<TripAggregate>> GetTripsByDriverIdAsync(int driverId)
    {
        return await _context.Set<TripAggregate>()
            .Where(t => t.DriverId == driverId)
            .Include(t => t.Alerts)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripAggregate>> GetTripsByVehicleIdAsync(int vehicleId)
    {
        return await _context.Set<TripAggregate>()
            .Where(t => t.VehicleId == vehicleId)
            .Include(t => t.Alerts)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TripAggregate?> GetActiveTripByDriverIdAsync(int driverId)
    {
        return await _context.Set<TripAggregate>()
            .Where(t => t.DriverId == driverId && (int)t.Status == 2) // Status.InProgress = 2
            .Include(t => t.Alerts)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TripAggregate>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Set<TripAggregate>()
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .Include(t => t.Alerts)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}

