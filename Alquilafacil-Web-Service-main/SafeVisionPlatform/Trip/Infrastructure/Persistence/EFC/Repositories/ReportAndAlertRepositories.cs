using SafeVisionPlatform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Repositories;

namespace SafeVisionPlatform.Trip.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación responsable de almacenar y recuperar los reportes
/// generados al finalizar un viaje.
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Report entity)
    {
        await _context.Set<Report>().AddAsync(entity);
    }

    public async Task<Report?> FindByIdAsync(int id)
    {
        return await _context.Set<Report>().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Report>> ListAsync()
    {
        return await _context.Set<Report>()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public void Update(Report entity)
    {
        _context.Set<Report>().Update(entity);
    }

    public void Remove(Report entity)
    {
        _context.Set<Report>().Remove(entity);
    }

    public async Task<IEnumerable<Report>> GetReportsByDriverIdAsync(int driverId)
    {
        return await _context.Set<Report>()
            .Where(r => r.DriverId == driverId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByStatusAsync(int status)
    {
        return await _context.Set<Report>()
            .Where(r => (int)r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Report?> GetReportByTripIdAsync(int tripId)
    {
        return await _context.Set<Report>()
            .FirstOrDefaultAsync(r => r.TripId == tripId);
    }
}

/// <summary>
/// Implementación del repositorio de alertas de viaje.
/// </summary>
public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;

    public AlertRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Alert entity)
    {
        await _context.Set<Alert>().AddAsync(entity);
    }

    public async Task<Alert?> FindByIdAsync(int id)
    {
        return await _context.Set<Alert>().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Alert>> ListAsync()
    {
        return await _context.Set<Alert>()
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }

    public void Update(Alert entity)
    {
        _context.Set<Alert>().Update(entity);
    }

    public async Task UpdateAsync(Alert alert)
    {
        _context.Set<Alert>().Update(alert);
        await Task.CompletedTask;
    }

    public void Remove(Alert entity)
    {
        _context.Set<Alert>().Remove(entity);
    }

    public async Task<IEnumerable<Alert>> GetAlertsByTripIdAsync(int tripId)
    {
        return await _context.Set<Alert>()
            .Where(a => a.TripId == tripId)
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsByTypeAndDateRangeAsync(int alertType, DateTime startDate, DateTime endDate)
    {
        return await _context.Set<Alert>()
            .Where(a => (int)a.AlertType == alertType && a.DetectedAt >= startDate && a.DetectedAt <= endDate)
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }
}

