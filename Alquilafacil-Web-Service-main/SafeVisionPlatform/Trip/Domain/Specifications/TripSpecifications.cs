using SafeVisionPlatform.Trip.Domain.Model.ValueObjects;
using SafeVisionPlatform.Trip.Domain.Model.Aggregates;

namespace SafeVisionPlatform.Trip.Domain.Specifications;

/// <summary>
/// Especificaciones para consultas de viajes.
/// </summary>
public static class TripSpecifications
{
    /// <summary>
    /// Obtiene los viajes activos (en progreso) de un conductor.
    /// </summary>
    public static Func<TripAggregate, bool> GetActiveTripsByDriver(int driverId)
    {
        return trip => trip.DriverId == driverId && (int)trip.Status == 2; // Status.InProgress = 2
    }

    /// <summary>
    /// Obtiene los viajes completados en un rango de fechas.
    /// </summary>
    public static Func<TripAggregate, bool> GetCompletedTripsInDateRange(DateTime startDate, DateTime endDate)
    {
        return trip => (int)trip.Status == 3 && // Status.Completed = 3
                       trip.CreatedAt >= startDate && 
                       trip.CreatedAt <= endDate;
    }

    /// <summary>
    /// Obtiene los viajes de un conductor con alto número de alertas críticas.
    /// </summary>
    public static Func<TripAggregate, bool> GetTripsWithCriticalAlerts(int driverId)
    {
        return trip => trip.DriverId == driverId && 
                       trip.Alerts.Count(a => a.Severity.HasValue && a.Severity > 0.7) > 0;
    }

    /// <summary>
    /// Obtiene los viajes cancelados.
    /// </summary>
    public static Func<TripAggregate, bool> GetCancelledTrips()
    {
        return trip => (int)trip.Status == 4; // Status.Cancelled = 4
    }
}

