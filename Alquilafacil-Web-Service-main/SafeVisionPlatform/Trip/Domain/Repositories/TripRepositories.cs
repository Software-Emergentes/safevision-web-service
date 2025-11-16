using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Trip.Domain.Repositories;

/// <summary>
/// Define las operaciones de persistencia del agregado Trip.
/// </summary>
public interface ITripRepository : IBaseRepository<TripAggregate>
{
    /// <summary>
    /// Obtiene el historial de viajes realizados por un conductor.
    /// </summary>
    Task<IEnumerable<TripAggregate>> GetTripsByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene todos los viajes asociados a un vehículo.
    /// </summary>
    Task<IEnumerable<TripAggregate>> GetTripsByVehicleIdAsync(int vehicleId);

    /// <summary>
    /// Obtiene el viaje activo (en progreso) de un conductor.
    /// </summary>
    Task<TripAggregate?> GetActiveTripByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene los viajes en un rango de fechas.
    /// </summary>
    Task<IEnumerable<TripAggregate>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Define las operaciones de persistencia para reportes de viaje.
/// </summary>
public interface IReportRepository : IBaseRepository<Report>
{
    /// <summary>
    /// Obtiene los reportes de un conductor específico.
    /// </summary>
    Task<IEnumerable<Report>> GetReportsByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene los reportes por estado.
    /// </summary>
    Task<IEnumerable<Report>> GetReportsByStatusAsync(int status);

    /// <summary>
    /// Obtiene el reporte asociado a un viaje.
    /// </summary>
    Task<Report?> GetReportByTripIdAsync(int tripId);
}

/// <summary>
/// Define las operaciones de persistencia para alertas de viaje.
/// </summary>
public interface IAlertRepository : IBaseRepository<Alert>
{
    /// <summary>
    /// Obtiene todas las alertas de un viaje específico.
    /// </summary>
    Task<IEnumerable<Alert>> GetAlertsByTripIdAsync(int tripId);

    /// <summary>
    /// Obtiene las alertas de un tipo específico en un rango de fechas.
    /// </summary>
    Task<IEnumerable<Alert>> GetAlertsByTypeAndDateRangeAsync(int alertType, DateTime startDate, DateTime endDate);
}

