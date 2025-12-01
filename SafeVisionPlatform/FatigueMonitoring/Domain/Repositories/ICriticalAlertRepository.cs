using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar alertas críticas de fatiga.
/// </summary>
public interface ICriticalAlertRepository : IBaseRepository<CriticalAlert>
{
    /// <summary>
    /// Obtiene alertas por conductor.
    /// </summary>
    Task<IEnumerable<CriticalAlert>> GetByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene alertas por viaje.
    /// </summary>
    Task<IEnumerable<CriticalAlert>> GetByTripIdAsync(int tripId);

    /// <summary>
    /// Obtiene alertas por gerente.
    /// </summary>
    Task<IEnumerable<CriticalAlert>> GetByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene alertas pendientes (no reconocidas).
    /// </summary>
    Task<IEnumerable<CriticalAlert>> GetPendingAlertsAsync();

    /// <summary>
    /// Obtiene alertas pendientes por gerente.
    /// </summary>
    Task<IEnumerable<CriticalAlert>> GetPendingAlertsByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene alertas en un rango de fechas.
    /// </summary>
    Task<IEnumerable<CriticalAlert>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Actualiza una alerta.
    /// </summary>
    Task UpdateAsync(CriticalAlert alert);
}
