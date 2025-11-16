using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;
using SafeVisionPlatform.Trip.Interfaces.REST.Transform;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de dashboard de flota.
/// Genera datos agregados y en tiempo real sobre el estado de la flota.
/// </summary>
public class FleetDashboardService : IFleetDashboardService
{
    private readonly ITripRepository _tripRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly ITripQueryService _tripQueryService;
    private readonly IAlertQueryService _alertQueryService;

    public FleetDashboardService(
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

    public async Task<FleetDashboardDTO> GetFleetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Obtener todos los viajes activos
        var allTrips = await _tripRepository.ListAsync();
        var activeTrips = allTrips.Where(t => t.Status == "InProgress").ToList();

        // Obtener viajes completados hoy
        var todayTrips = await _tripQueryService.GetTripsByDateRangeAsync(today, tomorrow);
        var todayCompletedTrips = todayTrips.Where(t => t.Status == "Completed").ToList();

        // Obtener todas las alertas de hoy
        var todayAlerts = await GetTodayAlertsAsync();

        // Construir lista de viajes activos resumidos
        var activeTripSummaries = activeTrips.Select(trip =>
        {
            var tripAlerts = todayAlerts.Where(a => a.TripId == trip.Id).ToList();
            var criticalAlerts = tripAlerts.Count(a => IsCriticalAlertType(a.AlertType));

            return new ActiveTripSummaryDTO
            {
                TripId = trip.Id,
                DriverId = trip.DriverId,
                DriverName = $"Conductor #{trip.DriverId}", // TODO: Obtener nombre real del conductor
                VehicleId = trip.VehicleId,
                StartTime = trip.Time.StartTime,
                DurationMinutes = (int)(DateTime.UtcNow - trip.Time.StartTime).TotalMinutes,
                AlertCount = tripAlerts.Count,
                CriticalAlertsCount = criticalAlerts,
                Status = trip.Status
            };
        }).ToList();

        // Identificar conductores en riesgo
        var driversAtRisk = await GetDriversAtRiskAsync();

        // Calcular estadísticas
        var statistics = await GetFleetStatisticsAsync(today, tomorrow);

        return new FleetDashboardDTO
        {
            ActiveDriversCount = activeTrips.Select(t => t.DriverId).Distinct().Count(),
            ActiveTripsCount = activeTrips.Count,
            TodayAlertsCount = todayAlerts.Count,
            TodayCompletedTripsCount = todayCompletedTrips.Count,
            ActiveTrips = activeTripSummaries,
            DriversAtRisk = driversAtRisk,
            Statistics = statistics
        };
    }

    public async Task<IEnumerable<DriverAtRiskDTO>> GetDriversAtRiskAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Obtener viajes activos
        var allTrips = await _tripRepository.ListAsync();
        var activeTrips = allTrips.Where(t => t.Status == "InProgress").ToList();

        // Obtener alertas de hoy
        var todayAlerts = await GetTodayAlertsAsync();

        var driversAtRisk = new List<DriverAtRiskDTO>();

        foreach (var trip in activeTrips)
        {
            var tripAlerts = todayAlerts.Where(a => a.TripId == trip.Id).ToList();
            var criticalAlerts = tripAlerts.Where(a => IsCriticalAlertType(a.AlertType)).ToList();

            if (criticalAlerts.Any())
            {
                var lastAlert = tripAlerts.OrderByDescending(a => a.Timestamp).FirstOrDefault();
                var riskLevel = DetermineRiskLevel(criticalAlerts.Count);

                driversAtRisk.Add(new DriverAtRiskDTO
                {
                    DriverId = trip.DriverId,
                    DriverName = $"Conductor #{trip.DriverId}",
                    TripId = trip.Id,
                    CriticalAlertsCount = criticalAlerts.Count,
                    LastAlertTime = lastAlert?.Timestamp ?? DateTime.UtcNow,
                    RiskLevel = riskLevel
                });
            }
        }

        return driversAtRisk.OrderByDescending(d => d.CriticalAlertsCount);
    }

    public async Task<FleetStatisticsDTO> GetFleetStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var trips = await _tripQueryService.GetTripsByDateRangeAsync(startDate, endDate);
        var tripsList = trips.ToList();
        var completedTrips = tripsList.Where(t => t.Status == "Completed").ToList();

        // Obtener alertas en el rango de fechas
        var alerts = await GetAlertsInRangeAsync(startDate, endDate);

        var totalDistance = completedTrips.Sum(t => t.DataPolicy.TotalDistanceKm);
        var totalDuration = completedTrips.Sum(t => t.DataPolicy.TotalDurationMinutes);
        var totalAlerts = alerts.Count();

        var averageAlerts = completedTrips.Any()
            ? (double)totalAlerts / completedTrips.Count
            : 0;

        var safeTrips = completedTrips.Count(t =>
        {
            var tripAlerts = alerts.Where(a => a.TripId == t.Id).ToList();
            return !tripAlerts.Any(a => IsCriticalAlertType(a.AlertType));
        });

        var safeTripsPercentage = completedTrips.Any()
            ? (double)safeTrips / completedTrips.Count * 100
            : 0;

        var uniqueDrivers = tripsList.Select(t => t.DriverId).Distinct().Count();

        return new FleetStatisticsDTO
        {
            TotalDistanceToday = totalDistance,
            TotalDrivingMinutesToday = totalDuration,
            AverageAlertsPerTripToday = averageAlerts,
            SafeTripsPercentage = safeTripsPercentage,
            UniqueDriversToday = uniqueDrivers
        };
    }

    private async Task<List<AlertDTO>> GetTodayAlertsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        return await GetAlertsInRangeAsync(today, tomorrow);
    }

    private async Task<List<AlertDTO>> GetAlertsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var allAlerts = await _alertRepository.ListAsync();
        return allAlerts
            .Where(a => a.Timestamp >= startDate && a.Timestamp < endDate)
            .Select(a => AlertAssembler.ToDTO(a))
            .ToList();
    }

    private bool IsCriticalAlertType(int alertType)
    {
        // Tipos críticos: 0=Drowsiness, 3=MicroSleep
        // Tipos no críticos: 1=Distraction, 2=Yawning, 4=Other
        return alertType == 0 || alertType == 3;
    }

    private string DetermineRiskLevel(int criticalAlertsCount)
    {
        return criticalAlertsCount switch
        {
            >= 5 => "Critical",
            >= 3 => "High",
            >= 2 => "Medium",
            _ => "Low"
        };
    }
}
