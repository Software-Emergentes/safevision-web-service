using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Management.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar eventos críticos.
/// </summary>
public interface ICriticalEventRepository : IBaseRepository<CriticalEvent>
{
    /// <summary>
    /// Obtiene eventos por conductor.
    /// </summary>
    Task<IEnumerable<CriticalEvent>> GetByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene eventos por gerente asignado.
    /// </summary>
    Task<IEnumerable<CriticalEvent>> GetByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene eventos por estado.
    /// </summary>
    Task<IEnumerable<CriticalEvent>> GetByStatusAsync(CriticalEventStatus status);

    /// <summary>
    /// Obtiene eventos pendientes (no resueltos).
    /// </summary>
    Task<IEnumerable<CriticalEvent>> GetPendingEventsAsync();

    /// <summary>
    /// Obtiene eventos en un rango de fechas.
    /// </summary>
    Task<IEnumerable<CriticalEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Actualiza un evento.
    /// </summary>
    Task UpdateAsync(CriticalEvent criticalEvent);
}
