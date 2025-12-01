using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de detección de patrones de riesgo.
/// </summary>
public class RiskPatternDetectionService : IRiskPatternDetectionService
{
    private readonly ITripQueryService _tripQueryService;
    private readonly IAlertRepository _alertRepository;
    private readonly ICriticalNotificationService _notificationService;
    private readonly ILogger<RiskPatternDetectionService> _logger;

    public RiskPatternDetectionService(
        ITripQueryService tripQueryService,
        IAlertRepository alertRepository,
        ICriticalNotificationService notificationService,
        ILogger<RiskPatternDetectionService> logger)
    {
        _tripQueryService = tripQueryService;
        _alertRepository = alertRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<DriverRiskAnalysisDTO> AnalyzeDriverRiskPatternsAsync(RiskPatternAnalysisRequestDTO request)
    {
        if (!request.DriverId.HasValue)
            throw new ArgumentException("DriverId is required for individual driver analysis");

        var driverId = request.DriverId.Value;

        _logger.LogInformation($"Analizando patrones de riesgo para conductor {driverId}");

        // Obtener viajes del conductor
        var allTrips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var trips = allTrips.Where(t =>
            t.StartTime >= request.StartDate &&
            t.StartTime <= request.EndDate &&
            t.StatusString == "Completed").ToList();

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
                DetectedAt = a.DetectedAt,
                Acknowledged = a.Acknowledged
            }));
        }

        var analysis = new DriverRiskAnalysisDTO
        {
            DriverId = driverId,
            DriverName = $"Conductor #{driverId}",
            AnalysisStartDate = request.StartDate,
            AnalysisEndDate = request.EndDate,
            TotalTripsAnalyzed = trips.Count,
            TotalAlertsAnalyzed = allAlerts.Count
        };

        // Analizar patrones por horario
        if (request.IncludeTimeOfDayAnalysis)
        {
            analysis.TimeOfDayPatterns = await AnalyzeTimeOfDayPatterns(driverId, trips, allAlerts);
            var riskyHours = analysis.TimeOfDayPatterns
                .Where(p => p.RiskScore >= request.MinimumRiskScore && p.TripsInThisTimeSlot >= request.MinimumOccurrences)
                .OrderByDescending(p => p.RiskScore)
                .Take(3)
                .Select(p => p.HourOfDay)
                .ToList();
            analysis.MostRiskyHours = riskyHours;
        }

        // Analizar patrones por día de la semana
        if (request.IncludeDayOfWeekAnalysis)
        {
            analysis.DayOfWeekPatterns = await AnalyzeDayOfWeekPatterns(driverId, trips, allAlerts);
            var riskyDays = analysis.DayOfWeekPatterns
                .Where(p => p.RiskScore >= request.MinimumRiskScore && p.TripsOnThisDay >= request.MinimumOccurrences)
                .OrderByDescending(p => p.RiskScore)
                .Take(2)
                .Select(p => p.DayOfWeek)
                .ToList();
            analysis.MostRiskyDaysOfWeek = riskyDays;
        }

        // Analizar patrones por duración de viaje
        if (request.IncludeDurationAnalysis)
        {
            analysis.DurationPatterns = await AnalyzeTripDurationPatterns(driverId, trips, allAlerts);
        }

        // Detectar patrones específicos
        analysis.DetectedPatterns = DetectSpecificPatterns(trips, allAlerts, request.MinimumOccurrences);

        // Calcular riesgo general y generar recomendaciones
        CalculateOverallRisk(analysis);
        GenerateRecommendations(analysis);

        return analysis;
    }

    public async Task<FleetRiskPatternsDTO> AnalyzeFleetRiskPatternsAsync(RiskPatternAnalysisRequestDTO request)
    {
        _logger.LogInformation("Analizando patrones de riesgo de toda la flota");

        var allTrips = await _tripQueryService.GetTripsByDateRangeAsync(request.StartDate, request.EndDate);
        var driverIds = allTrips.Select(t => t.DriverId).Distinct().ToList();

        var fleetAnalysis = new FleetRiskPatternsDTO
        {
            AnalysisStartDate = request.StartDate,
            AnalysisEndDate = request.EndDate,
            TotalDriversAnalyzed = driverIds.Count
        };

        // Analizar cada conductor
        foreach (var driverId in driverIds)
        {
            var driverRequest = new RiskPatternAnalysisRequestDTO
            {
                DriverId = driverId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MinimumOccurrences = request.MinimumOccurrences,
                MinimumRiskScore = request.MinimumRiskScore,
                IncludeTimeOfDayAnalysis = request.IncludeTimeOfDayAnalysis,
                IncludeDayOfWeekAnalysis = request.IncludeDayOfWeekAnalysis,
                IncludeDurationAnalysis = request.IncludeDurationAnalysis
            };

            var driverAnalysis = await AnalyzeDriverRiskPatternsAsync(driverRequest);
            fleetAnalysis.DriverAnalyses.Add(driverAnalysis);

            if (driverAnalysis.OverallRiskScore >= 70.0)
            {
                fleetAnalysis.DriversWithHighRiskPatterns++;
            }

            fleetAnalysis.TotalPatternsDetected += driverAnalysis.DetectedPatterns.Count;
        }

        // Calcular patrones comunes a nivel de flota
        CalculateFleetCommonPatterns(fleetAnalysis);

        return fleetAnalysis;
    }

    public async Task<IEnumerable<DriverRiskAnalysisDTO>> GetHighRiskDriversAsync(
        DateTime startDate,
        DateTime endDate,
        double minRiskScore = 70.0)
    {
        var request = new RiskPatternAnalysisRequestDTO
        {
            StartDate = startDate,
            EndDate = endDate,
            MinimumOccurrences = 3,
            MinimumRiskScore = minRiskScore
        };

        var fleetAnalysis = await AnalyzeFleetRiskPatternsAsync(request);

        return fleetAnalysis.DriverAnalyses
            .Where(d => d.OverallRiskScore >= minRiskScore)
            .OrderByDescending(d => d.OverallRiskScore);
    }

    public async Task<IEnumerable<TimeOfDayRiskPatternDTO>> GetTimeOfDayPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var allTrips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var trips = allTrips.Where(t =>
            t.StartTime >= startDate &&
            t.StartTime <= endDate &&
            t.StatusString == "Completed").ToList();

        var allAlerts = new List<AlertDTO>();
        foreach (var trip in trips)
        {
            var tripAlerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            allAlerts.AddRange(tripAlerts.Select(a => new AlertDTO
            {
                Id = a.Id,
                TripId = a.TripId,
                AlertType = (int)a.AlertType,
                DetectedAt = a.DetectedAt
            }));
        }

        return await AnalyzeTimeOfDayPatterns(driverId, trips, allAlerts);
    }

    public async Task<IEnumerable<DayOfWeekRiskPatternDTO>> GetDayOfWeekPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var allTrips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var trips = allTrips.Where(t =>
            t.StartTime >= startDate &&
            t.StartTime <= endDate &&
            t.StatusString == "Completed").ToList();

        var allAlerts = new List<AlertDTO>();
        foreach (var trip in trips)
        {
            var tripAlerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            allAlerts.AddRange(tripAlerts.Select(a => new AlertDTO
            {
                Id = a.Id,
                TripId = a.TripId,
                AlertType = (int)a.AlertType,
                DetectedAt = a.DetectedAt
            }));
        }

        return await AnalyzeDayOfWeekPatterns(driverId, trips, allAlerts);
    }

    public async Task<IEnumerable<TripDurationRiskPatternDTO>> GetTripDurationPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var allTrips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        var trips = allTrips.Where(t =>
            t.StartTime >= startDate &&
            t.StartTime <= endDate &&
            t.StatusString == "Completed").ToList();

        var allAlerts = new List<AlertDTO>();
        foreach (var trip in trips)
        {
            var tripAlerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            allAlerts.AddRange(tripAlerts.Select(a => new AlertDTO
            {
                Id = a.Id,
                TripId = a.TripId,
                AlertType = (int)a.AlertType
            }));
        }

        return await AnalyzeTripDurationPatterns(driverId, trips, allAlerts);
    }

    public async Task NotifyManagersAboutRiskPatternsAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        var request = new RiskPatternAnalysisRequestDTO
        {
            DriverId = driverId,
            StartDate = startDate,
            EndDate = endDate,
            MinimumOccurrences = 3,
            MinimumRiskScore = 70.0
        };

        var analysis = await AnalyzeDriverRiskPatternsAsync(request);

        if (analysis.OverallRiskScore >= 70.0 && analysis.DetectedPatterns.Any())
        {
            var message = $"PATRÓN DE RIESGO DETECTADO: Conductor #{driverId} muestra patrones recurrentes de fatiga. " +
                         $"Puntuación de riesgo: {analysis.OverallRiskScore:F1}/100. " +
                         $"Patrones detectados: {analysis.DetectedPatterns.Count}. " +
                         $"Se recomienda revisar horarios y asignar descansos adicionales.";

            var notificationDto = new CreateCriticalNotificationDTO
            {
                DriverId = driverId,
                TripId = 0, // No es específico de un viaje
                ManagerId = null, // Enviar a todos los gerentes
                Severity = analysis.OverallRiskScore >= 85 ? "Critical" : "High",
                AlertType = 0,
                CriticalAlertsCount = analysis.DetectedPatterns.Count,
                Message = message,
                Channel = "InApp"
            };

            await _notificationService.CreateAndSendCriticalNotificationAsync(notificationDto);

            _logger.LogInformation($"Notificación de patrón de riesgo enviada para conductor {driverId}");
        }
    }

    // ==================== MÉTODOS PRIVADOS DE ANÁLISIS ====================

    private async Task<List<TimeOfDayRiskPatternDTO>> AnalyzeTimeOfDayPatterns(
        int driverId,
        List<TripDTO> trips,
        List<AlertDTO> alerts)
    {
        var patterns = new List<TimeOfDayRiskPatternDTO>();

        // Agrupar por hora del día
        for (int hour = 0; hour < 24; hour++)
        {
            var tripsInHour = trips.Where(t => t.StartTime.Hour == hour).ToList();
            if (!tripsInHour.Any()) continue;

            var alertsInHour = alerts.Where(a => a.Timestamp.Hour == hour).ToList();
            var criticalAlerts = alertsInHour.Where(a => a.AlertType == 0 || a.AlertType == 3).ToList();

            var alertsPerTrip = tripsInHour.Count > 0 ? (double)alertsInHour.Count / tripsInHour.Count : 0;

            // Calcular puntuación de riesgo (0-100)
            var riskScore = CalculateRiskScore(alertsInHour.Count, criticalAlerts.Count, tripsInHour.Count);

            patterns.Add(new TimeOfDayRiskPatternDTO
            {
                DriverId = driverId,
                DriverName = $"Conductor #{driverId}",
                HourOfDay = hour,
                TimeRange = $"{hour:D2}:00-{(hour + 1) % 24:D2}:00",
                TotalAlerts = alertsInHour.Count,
                CriticalAlerts = criticalAlerts.Count,
                TripsInThisTimeSlot = tripsInHour.Count,
                AlertsPerTrip = alertsPerTrip,
                RiskScore = riskScore,
                RiskLevel = GetRiskLevel(riskScore)
            });
        }

        return await Task.FromResult(patterns.OrderByDescending(p => p.RiskScore).ToList());
    }

    private async Task<List<DayOfWeekRiskPatternDTO>> AnalyzeDayOfWeekPatterns(
        int driverId,
        List<TripDTO> trips,
        List<AlertDTO> alerts)
    {
        var patterns = new List<DayOfWeekRiskPatternDTO>();
        var dayNames = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };

        for (int day = 0; day < 7; day++)
        {
            var tripsOnDay = trips.Where(t => (int)t.StartTime.DayOfWeek == day).ToList();
            if (!tripsOnDay.Any()) continue;

            var tripIds = tripsOnDay.Select(t => t.Id).ToHashSet();
            var alertsOnDay = alerts.Where(a => tripIds.Contains(a.TripId)).ToList();
            var criticalAlerts = alertsOnDay.Where(a => a.AlertType == 0 || a.AlertType == 3).ToList();

            var alertsPerTrip = tripsOnDay.Count > 0 ? (double)alertsOnDay.Count / tripsOnDay.Count : 0;
            var riskScore = CalculateRiskScore(alertsOnDay.Count, criticalAlerts.Count, tripsOnDay.Count);

            patterns.Add(new DayOfWeekRiskPatternDTO
            {
                DriverId = driverId,
                DriverName = $"Conductor #{driverId}",
                DayOfWeek = day,
                DayName = dayNames[day],
                TotalAlerts = alertsOnDay.Count,
                CriticalAlerts = criticalAlerts.Count,
                TripsOnThisDay = tripsOnDay.Count,
                AlertsPerTrip = alertsPerTrip,
                RiskScore = riskScore,
                RiskLevel = GetRiskLevel(riskScore)
            });
        }

        return await Task.FromResult(patterns.OrderByDescending(p => p.RiskScore).ToList());
    }

    private async Task<List<TripDurationRiskPatternDTO>> AnalyzeTripDurationPatterns(
        int driverId,
        List<TripDTO> trips,
        List<AlertDTO> alerts)
    {
        var patterns = new List<TripDurationRiskPatternDTO>();
        var durationRanges = new[]
        {
            (0, 60, "0-60 min"),
            (60, 120, "1-2 horas"),
            (120, 180, "2-3 horas"),
            (180, 240, "3-4 horas"),
            (240, int.MaxValue, "4+ horas")
        };

        foreach (var (minDuration, maxDuration, label) in durationRanges)
        {
            var tripsInRange = trips.Where(t =>
                t.DataPolicy.TotalDurationMinutes >= minDuration &&
                t.DataPolicy.TotalDurationMinutes < maxDuration).ToList();

            if (!tripsInRange.Any()) continue;

            var tripIds = tripsInRange.Select(t => t.Id).ToHashSet();
            var alertsInRange = alerts.Where(a => tripIds.Contains(a.TripId)).ToList();

            var alertsPerTrip = tripsInRange.Count > 0 ? (double)alertsInRange.Count / tripsInRange.Count : 0;
            var criticalAlerts = alertsInRange.Where(a => a.AlertType == 0 || a.AlertType == 3).Count();
            var riskScore = CalculateRiskScore(alertsInRange.Count, criticalAlerts, tripsInRange.Count);

            patterns.Add(new TripDurationRiskPatternDTO
            {
                DriverId = driverId,
                DriverName = $"Conductor #{driverId}",
                DurationRangeMinutes = minDuration,
                DurationRange = label,
                TotalTrips = tripsInRange.Count,
                TotalAlerts = alertsInRange.Count,
                AlertsPerTrip = alertsPerTrip,
                RiskScore = riskScore,
                RiskLevel = GetRiskLevel(riskScore)
            });
        }

        return await Task.FromResult(patterns.OrderByDescending(p => p.RiskScore).ToList());
    }

    private List<RiskPatternDTO> DetectSpecificPatterns(
        List<TripDTO> trips,
        List<AlertDTO> alerts,
        int minOccurrences)
    {
        var patterns = new List<RiskPatternDTO>();

        // Patrón: Alertas recurrentes en horarios nocturnos (22:00-06:00)
        var nightAlerts = alerts.Where(a => a.Timestamp.Hour >= 22 || a.Timestamp.Hour < 6).ToList();
        if (nightAlerts.Count >= minOccurrences)
        {
            patterns.Add(new RiskPatternDTO
            {
                PatternType = "TimeOfDay",
                Description = "Alertas recurrentes durante horario nocturno (22:00-06:00)",
                OccurrenceCount = nightAlerts.Count,
                Severity = nightAlerts.Count >= 10 ? "High" : "Medium",
                FirstDetected = nightAlerts.Min(a => a.Timestamp),
                LastDetected = nightAlerts.Max(a => a.Timestamp),
                PatternDetails = new Dictionary<string, object>
                {
                    { "TimeRange", "22:00-06:00" },
                    { "TotalAlerts", nightAlerts.Count },
                    { "CriticalAlerts", nightAlerts.Count(a => a.AlertType == 0 || a.AlertType == 3) }
                },
                Recommendations = new List<string>
                {
                    "Evitar asignar turnos nocturnos a este conductor",
                    "Si son necesarios turnos nocturnos, programar descansos más frecuentes",
                    "Considerar rotación de horarios"
                }
            });
        }

        // Patrón: Viajes largos con alta densidad de alertas
        var longTripsWithAlerts = trips.Where(t =>
            t.DataPolicy.TotalDurationMinutes >= 180).ToList();

        if (longTripsWithAlerts.Count >= minOccurrences)
        {
            var longTripIds = longTripsWithAlerts.Select(t => t.Id).ToHashSet();
            var longTripAlerts = alerts.Where(a => longTripIds.Contains(a.TripId)).ToList();
            var avgAlertsPerLongTrip = longTripsWithAlerts.Count > 0 ?
                (double)longTripAlerts.Count / longTripsWithAlerts.Count : 0;

            if (avgAlertsPerLongTrip >= 3)
            {
                patterns.Add(new RiskPatternDTO
                {
                    PatternType = "Duration",
                    Description = "Alta incidencia de alertas en viajes largos (3+ horas)",
                    OccurrenceCount = longTripsWithAlerts.Count,
                    Severity = avgAlertsPerLongTrip >= 5 ? "High" : "Medium",
                    FirstDetected = longTripsWithAlerts.Min(t => t.StartTime),
                    LastDetected = longTripsWithAlerts.Max(t => t.StartTime),
                    PatternDetails = new Dictionary<string, object>
                    {
                        { "LongTripsCount", longTripsWithAlerts.Count },
                        { "AverageAlertsPerTrip", avgAlertsPerLongTrip },
                        { "TotalAlerts", longTripAlerts.Count }
                    },
                    Recommendations = new List<string>
                    {
                        "Programar descansos obligatorios cada 2 horas en viajes largos",
                        "Considerar dividir rutas largas entre múltiples conductores",
                        "Evaluar limitar la duración máxima de viajes para este conductor"
                    }
                });
            }
        }

        return patterns;
    }

    private void CalculateOverallRisk(DriverRiskAnalysisDTO analysis)
    {
        if (analysis.TotalTripsAnalyzed == 0)
        {
            analysis.OverallRiskScore = 0;
            analysis.OverallRiskLevel = "Unknown";
            return;
        }

        var avgAlertsPerTrip = (double)analysis.TotalAlertsAnalyzed / analysis.TotalTripsAnalyzed;

        // Base score por densidad de alertas
        var baseScore = Math.Min(avgAlertsPerTrip * 10, 50);

        // Penalización por patrones detectados
        var patternPenalty = analysis.DetectedPatterns.Count * 10;

        // Penalización por horarios de alto riesgo
        var riskyHoursPenalty = analysis.MostRiskyHours.Count * 5;

        // Score total
        analysis.OverallRiskScore = Math.Min(baseScore + patternPenalty + riskyHoursPenalty, 100);
        analysis.OverallRiskLevel = GetRiskLevel(analysis.OverallRiskScore);
    }

    private void GenerateRecommendations(DriverRiskAnalysisDTO analysis)
    {
        var recommendations = new List<string>();

        if (analysis.OverallRiskScore >= 70)
        {
            recommendations.Add("Se recomienda revisión médica para descartar trastornos del sueño");
            recommendations.Add("Asignar descansos adicionales y monitoreo más frecuente");
        }

        if (analysis.MostRiskyHours.Any())
        {
            var hours = string.Join(", ", analysis.MostRiskyHours.Select(h => $"{h}:00"));
            recommendations.Add($"Evitar asignar viajes en horarios de alto riesgo: {hours}");
        }

        if (analysis.MostRiskyDaysOfWeek.Any())
        {
            var dayNames = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
            var days = string.Join(", ", analysis.MostRiskyDaysOfWeek.Select(d => dayNames[d]));
            recommendations.Add($"Programar descansos adicionales los días: {days}");
        }

        foreach (var pattern in analysis.DetectedPatterns)
        {
            recommendations.AddRange(pattern.Recommendations);
        }

        analysis.PrimaryRecommendations = recommendations.Distinct().Take(5).ToList();
    }

    private void CalculateFleetCommonPatterns(FleetRiskPatternsDTO fleetAnalysis)
    {
        // Calcular horas más riesgosas a nivel de flota
        var hourRiskScores = new Dictionary<int, List<double>>();
        foreach (var driver in fleetAnalysis.DriverAnalyses)
        {
            foreach (var pattern in driver.TimeOfDayPatterns)
            {
                if (!hourRiskScores.ContainsKey(pattern.HourOfDay))
                    hourRiskScores[pattern.HourOfDay] = new List<double>();

                hourRiskScores[pattern.HourOfDay].Add(pattern.RiskScore);
            }
        }

        fleetAnalysis.FleetMostRiskyHours = hourRiskScores
            .OrderByDescending(kvp => kvp.Value.Average())
            .Take(3)
            .Select(kvp => kvp.Key)
            .ToList();

        // Calcular días más riesgosos a nivel de flota
        var dayRiskScores = new Dictionary<int, List<double>>();
        foreach (var driver in fleetAnalysis.DriverAnalyses)
        {
            foreach (var pattern in driver.DayOfWeekPatterns)
            {
                if (!dayRiskScores.ContainsKey(pattern.DayOfWeek))
                    dayRiskScores[pattern.DayOfWeek] = new List<double>();

                dayRiskScores[pattern.DayOfWeek].Add(pattern.RiskScore);
            }
        }

        fleetAnalysis.FleetMostRiskyDays = dayRiskScores
            .OrderByDescending(kvp => kvp.Value.Average())
            .Take(2)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    private double CalculateRiskScore(int totalAlerts, int criticalAlerts, int totalTrips)
    {
        if (totalTrips == 0) return 0;

        var alertsPerTrip = (double)totalAlerts / totalTrips;
        var criticalRatio = totalAlerts > 0 ? (double)criticalAlerts / totalAlerts : 0;

        // Base score por densidad de alertas (0-60 puntos)
        var densityScore = Math.Min(alertsPerTrip * 15, 60);

        // Bonus por alertas críticas (0-40 puntos)
        var criticalScore = criticalRatio * 40;

        return Math.Min(densityScore + criticalScore, 100);
    }

    private string GetRiskLevel(double riskScore)
    {
        return riskScore switch
        {
            >= 85 => "Critical",
            >= 70 => "High",
            >= 50 => "Medium",
            >= 30 => "Low",
            _ => "Minimal"
        };
    }
}
