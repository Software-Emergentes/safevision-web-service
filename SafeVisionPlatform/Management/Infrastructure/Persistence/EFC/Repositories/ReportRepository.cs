using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Management.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.Management.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de reportes.
/// </summary>
public class ReportRepositoryImpl : BaseRepository<Report>, IReportRepository
{
    public ReportRepositoryImpl(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Report>> GetByGeneratedByIdAsync(int managerId)
    {
        return await Context.Set<Report>()
            .Where(r => r.GeneratedById == managerId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByDriverIdAsync(int driverId)
    {
        return await Context.Set<Report>()
            .Where(r => r.DriverId == driverId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByFleetIdAsync(int fleetId)
    {
        return await Context.Set<Report>()
            .Where(r => r.FleetId == fleetId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByReportTypeAsync(ReportType reportType)
    {
        return await Context.Set<Report>()
            .Where(r => r.ReportType == reportType)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Set<Report>()
            .Where(r => r.GeneratedAt >= startDate && r.GeneratedAt <= endDate)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Report report)
    {
        Context.Set<Report>().Update(report);
        await Task.CompletedTask;
    }
}
