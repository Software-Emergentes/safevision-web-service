using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;
using SafeVisionPlatform.Trip.Interfaces.REST.Transform;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de historial de conductor.
/// Genera análisis personalizados de viajes y alertas para cada conductor.
/// </summary>
public class DriverHistoryService : IDriverHistoryService
{
    private readonly ITripRepository _tripRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly ITripQueryService _tripQueryService;
    private readonly IAlertQueryService _alertQueryService;

    public DriverHistoryService(
        ITripRepository tripRepository,
        IAlertRepository alertRepository,
        ITripQueryService tripQueryService,
        IAlertQueryService alertQueryService)
    {
        _tripRepository = tripRepository;
        _alertRepository = alertRepository;
        _tripQueryService = tripQueryService;
        _alertQueryService = alertQueryService;
    }

    public async Task<DriverHistoryDTO> GetDriverHistoryAsync(int driverId)
    {
        // Obtener todos los viajes del conductor
        var trips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var tripsList = trips.ToList();

        // Obtener todas las alertas de esos viajes
        var allAlerts = new List<AlertDTO>();
        foreach (var trip in tripsList)
        {
            var tripAlerts = await _alertQueryService.GetAlertsByTripIdAsync(trip.Id);
            allAlerts.AddRange(tripAlerts);
        }

        return BuildDriverHistory(driverId, tripsList, allAlerts);
    }

    public async Task<DriverHistoryDTO> GetDriverHistoryByDateRangeAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        // Obtener viajes del conductor en el rango de fechas
        var allTrips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var tripsList = allTrips
            .Where(t => t.StartTime >= startDate && t.StartTime <= endDate)
            .ToList();

        // Obtener alertas de esos viajes
        var allAlerts = new List<AlertDTO>();
        foreach (var trip in tripsList)
        {
            var tripAlerts = await _alertQueryService.GetAlertsByTripIdAsync(trip.Id);
            allAlerts.AddRange(tripAlerts.Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate));
        }

        return BuildDriverHistory(driverId, tripsList, allAlerts);
    }

    public async Task<FatiguePatternDTO> GetDriverFatiguePatternsAsync(int driverId)
    {
        var history = await GetDriverHistoryAsync(driverId);
        return history.FatiguePattern;
    }

    private DriverHistoryDTO BuildDriverHistory(int driverId, List<TripDTO> trips, List<AlertDTO> alerts)
    {
        var totalTrips = trips.Count;
        var totalAlerts = alerts.Count;
        var averageAlerts = totalTrips > 0 ? (double)totalAlerts / totalTrips : 0;

        var completedTrips = trips.Where(t => t.Status == "Completed").ToList();
        var totalDistance = completedTrips.Sum(t => t.DataPolicy.TotalDistanceKm);
        var totalDuration = completedTrips.Sum(t => t.DataPolicy.TotalDurationMinutes);

        // Distribución de alertas por tipo
        var alertsByType = alerts
            .GroupBy(a => a.AlertType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Análisis de patrones de fatiga
        var fatiguePattern = AnalyzeFatiguePatterns(trips, alerts);

        return new DriverHistoryDTO
        {
            DriverId = driverId,
            TotalTrips = totalTrips,
            TotalAlerts = totalAlerts,
            AverageAlertsPerTrip = averageAlerts,
            TotalDistanceKm = totalDistance,
            TotalDrivingMinutes = totalDuration,
            Trips = trips,
            Alerts = alerts,
            AlertsByType = alertsByType,
            FatiguePattern = fatiguePattern
        };
    }

    private FatiguePatternDTO AnalyzeFatiguePatterns(List<TripDTO> trips, List<AlertDTO> alerts)
    {
        if (!alerts.Any())
        {
            return new FatiguePatternDTO
            {
                MostAlertsHour = 0,
                AlertsInPeakHour = 0,
                HourlyDistribution = new List<HourlyAlertDistributionDTO>(),
                MostFrequentAlertType = 0,
                SafeTripsPercentage = 100.0
            };
        }

        // Distribución horaria de alertas
        var hourlyDistribution = alerts
            .GroupBy(a => a.Timestamp.Hour)
            .Select(g => new HourlyAlertDistributionDTO
            {
                Hour = g.Key,
                AlertCount = g.Count()
            })
            .OrderBy(h => h.Hour)
            .ToList();

        // Hora con más alertas
        var peakHour = hourlyDistribution.OrderByDescending(h => h.AlertCount).FirstOrDefault();
        var mostAlertsHour = peakHour?.Hour ?? 0;
        var alertsInPeakHour = peakHour?.AlertCount ?? 0;

        // Tipo de alerta más frecuente
        var mostFrequentType = alerts
            .GroupBy(a => a.AlertType)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        // Calcular porcentaje de viajes seguros
        var criticalAlertTypes = new[] { 0, 3 }; // Drowsiness, MicroSleep
        var tripIds = trips.Select(t => t.Id).ToHashSet();
        var tripsWithCriticalAlerts = alerts
            .Where(a => criticalAlertTypes.Contains(a.AlertType) && tripIds.Contains(a.TripId))
            .Select(a => a.TripId)
            .Distinct()
            .Count();

        var safeTrips = trips.Count - tripsWithCriticalAlerts;
        var safeTripsPercentage = trips.Any() ? (double)safeTrips / trips.Count * 100 : 100.0;

        return new FatiguePatternDTO
        {
            MostAlertsHour = mostAlertsHour,
            AlertsInPeakHour = alertsInPeakHour,
            HourlyDistribution = hourlyDistribution,
            MostFrequentAlertType = mostFrequentType,
            SafeTripsPercentage = safeTripsPercentage
        };
    }
}
