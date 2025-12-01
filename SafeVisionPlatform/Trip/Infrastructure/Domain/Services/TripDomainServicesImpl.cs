using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Infrastructure.Domain.Services;

/// <summary>
/// Implementación de ITripManagerService que coordina operaciones principales del viaje.
/// </summary>
public class TripManagerService : ITripManagerService
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<TripManagerService> _logger;

    public TripManagerService(ITripRepository tripRepository, ILogger<TripManagerService> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task<bool> ValidateDriverAndVehicleAvailabilityAsync(int driverId, int vehicleId)
    {
        try
        {
            // Verificar si el conductor tiene un viaje activo
            var activeTrip = await _tripRepository.GetActiveTripByDriverIdAsync(driverId);
            if (activeTrip != null)
            {
                _logger.LogWarning($"Conductor {driverId} ya tiene un viaje activo.");
                return false;
            }

            // Aquí se puede agregar validación con el contexto Driver
            // y verificar que el vehículo no esté en mantenimiento, etc.

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error validando disponibilidad: {ex.Message}");
            return false;
        }
    }

    public bool ValidateTripCanBeEnded(TripAggregate trip)
    {
        // Solo se pueden finalizar viajes en progreso
        return (int)trip.Status == 2; // Status.InProgress = 2
    }

    public bool ValidateTripCanBeCancelled(TripAggregate trip)
    {
        // No se pueden cancelar viajes finalizados o ya cancelados
        var status = (int)trip.Status;
        return status != 3 && status != 4; // No Completed ni Cancelled
    }
}

/// <summary>
/// Implementación de ITripReportGenerator que genera reportes de viaje.
/// </summary>
public class TripReportGenerator : ITripReportGenerator
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<TripReportGenerator> _logger;

    public TripReportGenerator(IReportRepository reportRepository, ILogger<TripReportGenerator> logger)
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    public async Task<Report> GenerateReportAsync(TripAggregate trip, double distanceKm, string? notes = null)
    {
        try
        {
            var metrics = await CalculateMetricsAsync(trip);

            var report = new Report(
                tripId: trip.Id,
                driverId: trip.DriverId,
                vehicleId: trip.VehicleId,
                startTime: trip.Time.StartTime,
                endTime: trip.Time.EndTime ?? DateTime.UtcNow,
                distanceKm: distanceKm,
                alertCount: trip.Alerts.Count,
                recipient: ReportRecipient.Both,
                notes: notes
            );

            _logger.LogInformation($"Reporte generado para viaje {trip.Id}");
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generando reporte para viaje {trip.Id}: {ex.Message}");
            throw;
        }
    }

    public async Task<TripMetrics> CalculateMetricsAsync(TripAggregate trip)
    {
        return await Task.FromResult(new TripMetrics
        {
            DurationMinutes = trip.GetDurationInMinutes(),
            DistanceKm = 0, // Será proporcionado por el servicio de ubicación
            AverageSpeed = 0, // Será calculado
            AlertCount = trip.Alerts.Count,
            CriticalAlertCount = trip.Alerts.Count(a => a.Severity.HasValue && a.Severity > 0.7),
            SafetyScore = CalculateSafetyScore(trip)
        });
    }

    private double CalculateSafetyScore(TripAggregate trip)
    {
        // Lógica para calcular puntuación de seguridad (0-100)
        // Basada en número y severidad de alertas
        const double baseScore = 100.0;
        var alertPenalty = trip.Alerts.Sum(a => a.Severity.HasValue ? a.Severity.Value * 10 : 5);
        return Math.Max(0, baseScore - alertPenalty);
    }
}

