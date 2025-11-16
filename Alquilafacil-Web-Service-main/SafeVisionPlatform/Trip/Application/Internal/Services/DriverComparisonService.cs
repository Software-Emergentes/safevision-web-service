using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de comparación de conductores.
/// </summary>
public class DriverComparisonService : IDriverComparisonService
{
    private readonly ITripRepository _tripRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly ITripQueryService _tripQueryService;
    private readonly ITripRecommendationService _recommendationService;

    public DriverComparisonService(
        ITripRepository tripRepository,
        IAlertRepository alertRepository,
        ITripQueryService tripQueryService,
        ITripRecommendationService recommendationService)
    {
        _tripRepository = tripRepository;
        _alertRepository = alertRepository;
        _tripQueryService = tripQueryService;
        _recommendationService = recommendationService;
    }

    public async Task<DriverComparisonDTO> CompareDriversAsync(IEnumerable<int> driverIds, DateTime startDate, DateTime endDate)
    {
        var driverMetrics = new List<DriverMetricsDTO>();

        foreach (var driverId in driverIds)
        {
            var metrics = await GetDriverMetricsAsync(driverId, startDate, endDate);
            driverMetrics.Add(metrics);
        }

        return BuildComparisonDTO(driverMetrics, startDate, endDate);
    }

    public async Task<DriverComparisonDTO> CompareAllDriversAsync(DateTime startDate, DateTime endDate)
    {
        var allTrips = await _tripQueryService.GetTripsByDateRangeAsync(startDate, endDate);
        var driverIds = allTrips.Select(t => t.DriverId).Distinct();

        return await CompareDriversAsync(driverIds, startDate, endDate);
    }

    public async Task<IEnumerable<DriverRankingDTO>> GetDriverRankingsAsync(DateTime startDate, DateTime endDate, int limit = 10)
    {
        var comparison = await CompareAllDriversAsync(startDate, endDate);
        return comparison.DriverRankings.Take(limit);
    }

    private async Task<DriverMetricsDTO> GetDriverMetricsAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        // Obtener viajes del conductor en el rango
        var allTrips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var trips = allTrips.Where(t => t.StartTime >= startDate && t.StartTime <= endDate).ToList();

        var completedTrips = trips.Where(t => t.Status == "Completed").ToList();

        // Obtener todas las alertas de estos viajes
        var allAlerts = new List<AlertDTO>();
        foreach (var trip in trips)
        {
            var tripAlerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            allAlerts.AddRange(tripAlerts.Select(a => new AlertDTO
            {
                Id = a.Id,
                TripId = a.TripId,
                AlertType = (int)a.AlertType,
                Description = a.Description,
                Severity = a.Severity,
                Timestamp = a.DetectedAt,
                Acknowledged = a.Acknowledged
            }));
        }

        // Calcular métricas
        var totalTrips = trips.Count;
        var totalAlerts = allAlerts.Count;
        var drowsinessAlerts = allAlerts.Count(a => a.AlertType == 0);
        var distractionAlerts = allAlerts.Count(a => a.AlertType == 1);
        var microSleepAlerts = allAlerts.Count(a => a.AlertType == 3);

        var totalDistance = completedTrips.Sum(t => t.DataPolicy.TotalDistanceKm);
        var totalDuration = completedTrips.Sum(t => t.DataPolicy.TotalDurationMinutes);

        var averageAlertsPerTrip = totalTrips > 0 ? (double)totalAlerts / totalTrips : 0;
        var alertsPerHour = totalDuration > 0 ? (double)totalAlerts / (totalDuration / 60.0) : 0;
        var alertsPerKm = totalDistance > 0 ? (double)totalAlerts / totalDistance : 0;

        // Calcular puntuación de seguridad promedio
        var safetyScores = new List<int>();
        foreach (var trip in completedTrips)
        {
            var score = await _recommendationService.CalculateSafetyScoreAsync(trip.Id);
            safetyScores.Add(score);
        }
        var avgSafetyScore = safetyScores.Any() ? (int)safetyScores.Average() : 0;

        // Viajes seguros (sin alertas críticas)
        var criticalAlertTypes = new[] { 0, 3 }; // Drowsiness, MicroSleep
        var safeTrips = trips.Count(t =>
        {
            var tripAlerts = allAlerts.Where(a => a.TripId == t.Id && criticalAlertTypes.Contains(a.AlertType)).ToList();
            return !tripAlerts.Any();
        });
        var safeTripsPercentage = totalTrips > 0 ? (double)safeTrips / totalTrips * 100 : 0;

        // Porcentaje de alertas críticas
        var criticalAlerts = drowsinessAlerts + microSleepAlerts;
        var criticalAlertsPercentage = totalAlerts > 0 ? (double)criticalAlerts / totalAlerts * 100 : 0;

        return new DriverMetricsDTO
        {
            DriverId = driverId,
            DriverName = $"Conductor #{driverId}",
            TotalTrips = totalTrips,
            CompletedTrips = completedTrips.Count,
            TotalDistanceKm = totalDistance,
            TotalDrivingMinutes = totalDuration,
            TotalAlerts = totalAlerts,
            DrowsinessAlerts = drowsinessAlerts,
            DistractionAlerts = distractionAlerts,
            MicroSleepAlerts = microSleepAlerts,
            AverageAlertsPerTrip = averageAlertsPerTrip,
            AlertsPerHour = alertsPerHour,
            AlertsPerKm = alertsPerKm,
            SafetyScore = avgSafetyScore,
            SafeTripsPercentage = safeTripsPercentage,
            CriticalAlertsPercentage = criticalAlertsPercentage
        };
    }

    private DriverComparisonDTO BuildComparisonDTO(List<DriverMetricsDTO> driverMetrics, DateTime startDate, DateTime endDate)
    {
        // Crear rankings
        var rankings = driverMetrics
            .OrderByDescending(d => d.SafetyScore)
            .ThenBy(d => d.TotalAlerts)
            .Select((d, index) => new DriverRankingDTO
            {
                Rank = index + 1,
                DriverId = d.DriverId,
                DriverName = d.DriverName,
                SafetyScore = d.SafetyScore,
                TotalTrips = d.TotalTrips,
                TotalAlerts = d.TotalAlerts,
                SafeTripsPercentage = d.SafeTripsPercentage
            })
            .ToList();

        // Calcular métricas agregadas
        var aggregate = new AggregateMetricsDTO
        {
            TotalDrivers = driverMetrics.Count,
            TotalTrips = driverMetrics.Sum(d => d.TotalTrips),
            TotalAlerts = driverMetrics.Sum(d => d.TotalAlerts),
            AverageSafetyScore = driverMetrics.Any() ? driverMetrics.Average(d => d.SafetyScore) : 0,
            AverageAlertsPerTrip = driverMetrics.Any() ? driverMetrics.Average(d => d.AverageAlertsPerTrip) : 0,
            BestSafetyScore = driverMetrics.Any() ? driverMetrics.Max(d => d.SafetyScore) : 0,
            WorstSafetyScore = driverMetrics.Any() ? driverMetrics.Min(d => d.SafetyScore) : 0,
            BestDriverId = rankings.FirstOrDefault()?.DriverId ?? 0,
            DriversMostImprovement = 0 // Requeriría datos históricos de comparación
        };

        return new DriverComparisonDTO
        {
            StartDate = startDate,
            EndDate = endDate,
            DriverMetrics = driverMetrics,
            AggregateMetrics = aggregate,
            DriverRankings = rankings
        };
    }
}
