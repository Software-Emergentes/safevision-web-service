using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar eventos de somnolencia.
/// </summary>
public interface IDrowsinessEventRepository : IBaseRepository<DrowsinessEvent>
{
    /// <summary>
    /// Obtiene eventos de somnolencia por conductor.
    /// </summary>
    Task<IEnumerable<DrowsinessEvent>> GetByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene eventos de somnolencia por viaje.
    /// </summary>
    Task<IEnumerable<DrowsinessEvent>> GetByTripIdAsync(int tripId);

    /// <summary>
    /// Obtiene eventos de somnolencia por sesión de monitoreo.
    /// </summary>
    Task<IEnumerable<DrowsinessEvent>> GetByMonitoringSessionIdAsync(int sessionId);

    /// <summary>
    /// Obtiene eventos no procesados.
    /// </summary>
    Task<IEnumerable<DrowsinessEvent>> GetUnprocessedEventsAsync();

    /// <summary>
    /// Obtiene eventos en un rango de fechas.
    /// </summary>
    Task<IEnumerable<DrowsinessEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Obtiene el conteo de eventos por tipo para un conductor.
    /// </summary>
    Task<Dictionary<DrowsinessEventType, int>> GetEventCountsByTypeForDriverAsync(int driverId, DateTime startDate, DateTime endDate);
}
