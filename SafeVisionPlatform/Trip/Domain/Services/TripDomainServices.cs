using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Coordina las operaciones principales del viaje (inicio, fin, cancelación),
/// aplicando reglas de negocio y validando los estados.
/// </summary>
public interface ITripManagerService
{
    /// <summary>
    /// Valida si un conductor y vehículo están disponibles para iniciar un viaje.
    /// </summary>
    Task<bool> ValidateDriverAndVehicleAvailabilityAsync(int driverId, int vehicleId);

    /// <summary>
    /// Valida si un viaje puede ser finalizado.
    /// </summary>
    bool ValidateTripCanBeEnded(TripAggregate trip);

    /// <summary>
    /// Valida si un viaje puede ser cancelado.
    /// </summary>
    bool ValidateTripCanBeCancelled(TripAggregate trip);
}

/// <summary>
/// Procesa la información del viaje y genera los reportes finales
/// para los usuarios correspondientes.
/// </summary>
public interface ITripReportGenerator
{
    /// <summary>
    /// Genera un reporte para un viaje completado.
    /// </summary>
    Task<Report> GenerateReportAsync(TripAggregate trip, double distanceKm, string? notes = null);

    /// <summary>
    /// Calcula las métricas del viaje.
    /// </summary>
    Task<TripMetrics> CalculateMetricsAsync(TripAggregate trip);
}

/// <summary>
/// Representa las métricas calculadas de un viaje.
/// </summary>
public class TripMetrics
{
    public int DurationMinutes { get; set; }
    public double DistanceKm { get; set; }
    public double AverageSpeed { get; set; }
    public int AlertCount { get; set; }
    public int CriticalAlertCount { get; set; }
    public double SafetyScore { get; set; } // Valor entre 0 y 100
}

