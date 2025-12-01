using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Management.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar reportes.
/// </summary>
public interface IReportRepository : IBaseRepository<Report>
{
    /// <summary>
    /// Obtiene reportes por gerente.
    /// </summary>
    Task<IEnumerable<Report>> GetByGeneratedByIdAsync(int managerId);

    /// <summary>
    /// Obtiene reportes por conductor.
    /// </summary>
    Task<IEnumerable<Report>> GetByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene reportes por flota.
    /// </summary>
    Task<IEnumerable<Report>> GetByFleetIdAsync(int fleetId);

    /// <summary>
    /// Obtiene reportes por tipo.
    /// </summary>
    Task<IEnumerable<Report>> GetByReportTypeAsync(ReportType reportType);

    /// <summary>
    /// Obtiene reportes en un rango de fechas.
    /// </summary>
    Task<IEnumerable<Report>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Actualiza un reporte.
    /// </summary>
    Task UpdateAsync(Report report);
}
