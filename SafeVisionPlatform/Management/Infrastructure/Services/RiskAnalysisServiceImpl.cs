using SafeVisionPlatform.Management.Domain.Model.ValueObjects;
using SafeVisionPlatform.Management.Domain.Services;
using SafeVisionPlatform.Trip.Domain.Repositories;

namespace SafeVisionPlatform.Management.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de análisis de riesgos.
/// </summary>
public class RiskAnalysisServiceImpl : IRiskAnalysisService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<RiskAnalysisServiceImpl> _logger;

    public RiskAnalysisServiceImpl(
        IAlertRepository alertRepository,
        ITripRepository tripRepository,
        ILogger<RiskAnalysisServiceImpl> logger)
    {
        _alertRepository = alertRepository;
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RiskPattern>> AnalyzeDriverRiskPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var patterns = new List<RiskPattern>();

        // Analizar patrones por hora del día
        var timePatterns = await IdentifyTimeOfDayPatternsAsync(driverId, startDate, endDate);
        patterns.AddRange(timePatterns);

        // Analizar patrones por día de la semana
        var dayPatterns = await IdentifyDayOfWeekPatternsAsync(driverId, startDate, endDate);
        patterns.AddRange(dayPatterns);

        // Agregar recomendaciones a cada patrón
        foreach (var pattern in patterns)
        {
            var recommendations = GeneratePatternRecommendations(pattern);
            foreach (var rec in recommendations)
            {
                pattern.AddRecommendation(rec);
            }
        }

        return patterns;
    }

    public async Task<IEnumerable<RiskPattern>> AnalyzeFleetRiskPatternsAsync(
        int fleetId,
        DateTime startDate,
        DateTime endDate)
    {
        // Obtener todos los viajes de la flota
        var trips = await _tripRepository.GetTripsByDateRangeAsync(startDate, endDate);
        var driverIds = trips.Select(t => t.DriverId).Distinct();

        var allPatterns = new List<RiskPattern>();

        foreach (var driverId in driverIds)
        {
            var driverPatterns = await AnalyzeDriverRiskPatternsAsync(driverId, startDate, endDate);
            allPatterns.AddRange(driverPatterns);
        }

        // Consolidar patrones similares
        var consolidatedPatterns = ConsolidatePatterns(allPatterns);

        return consolidatedPatterns;
    }

    public async Task<IEnumerable<RiskPattern>> IdentifyTimeOfDayPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var patterns = new List<RiskPattern>();
        var trips = await _tripRepository.GetTripsByDriverIdAsync(driverId);
        var filteredTrips = trips.Where(t =>
            t.Time.StartTime >= startDate && t.Time.StartTime <= endDate).ToList();

        // Obtener alertas de estos viajes
        var alertCounts = new Dictionary<int, int>(); // hora -> conteo
        foreach (var trip in filteredTrips)
        {
            var alerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            foreach (var alert in alerts)
            {
                var hour = alert.DetectedAt.Hour;
                if (!alertCounts.ContainsKey(hour))
                    alertCounts[hour] = 0;
                alertCounts[hour]++;
            }
        }

        // Identificar horas con alta concentración de alertas
        var avgAlerts = alertCounts.Values.Any() ? alertCounts.Values.Average() : 0;
        foreach (var kvp in alertCounts.Where(kv => kv.Value > avgAlerts * 1.5))
        {
            var pattern = new RiskPattern(
                RiskPatternType.TimeOfDay,
                $"Alta incidencia de alertas entre {kvp.Key}:00 y {kvp.Key + 1}:00",
                kvp.Value,
                Math.Min((double)kvp.Value / (avgAlerts + 1), 1.0));

            pattern.SetTimeRange(kvp.Key, kvp.Key + 1);
            patterns.Add(pattern);
        }

        return patterns;
    }

    public async Task<IEnumerable<RiskPattern>> IdentifyDayOfWeekPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var patterns = new List<RiskPattern>();
        var trips = await _tripRepository.GetTripsByDriverIdAsync(driverId);
        var filteredTrips = trips.Where(t =>
            t.Time.StartTime >= startDate && t.Time.StartTime <= endDate).ToList();

        var alertsByDay = new Dictionary<DayOfWeek, int>();

        foreach (var trip in filteredTrips)
        {
            var alerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            var day = trip.Time.StartTime.DayOfWeek;

            if (!alertsByDay.ContainsKey(day))
                alertsByDay[day] = 0;

            alertsByDay[day] += alerts.Count();
        }

        var avgAlerts = alertsByDay.Values.Any() ? alertsByDay.Values.Average() : 0;
        foreach (var kvp in alertsByDay.Where(kv => kv.Value > avgAlerts * 1.5))
        {
            var pattern = new RiskPattern(
                RiskPatternType.DayOfWeek,
                $"Alta incidencia de alertas los {GetDayName(kvp.Key)}",
                kvp.Value,
                Math.Min((double)kvp.Value / (avgAlerts + 1), 1.0));

            pattern.SetDaysOfWeek(new[] { kvp.Key });
            patterns.Add(pattern);
        }

        return patterns;
    }

    public IEnumerable<string> GenerateRecommendations(IEnumerable<RiskPattern> patterns)
    {
        var recommendations = new List<string>();

        foreach (var pattern in patterns)
        {
            recommendations.AddRange(GeneratePatternRecommendations(pattern));
        }

        return recommendations.Distinct();
    }

    private IEnumerable<string> GeneratePatternRecommendations(RiskPattern pattern)
    {
        var recommendations = new List<string>();

        switch (pattern.PatternType)
        {
            case RiskPatternType.TimeOfDay:
                if (pattern.StartHour >= 22 || pattern.StartHour < 6)
                {
                    recommendations.Add("Evitar asignar turnos nocturnos a este conductor");
                    recommendations.Add("Programar descansos adicionales en horarios de alto riesgo");
                }
                else
                {
                    recommendations.Add($"Monitorear con atención las horas {pattern.StartHour}:00-{pattern.EndHour}:00");
                }
                break;

            case RiskPatternType.DayOfWeek:
                recommendations.Add($"Considerar rotación de turnos los días de alto riesgo");
                recommendations.Add("Programar descansos adicionales en días identificados");
                break;

            case RiskPatternType.TripDuration:
                recommendations.Add("Limitar duración máxima de viajes");
                recommendations.Add("Programar paradas obligatorias cada 2 horas");
                break;
        }

        if (pattern.SeverityLevel == "Critical" || pattern.SeverityLevel == "High")
        {
            recommendations.Add("Se recomienda evaluación médica para descartar trastornos del sueño");
        }

        return recommendations;
    }

    private IEnumerable<RiskPattern> ConsolidatePatterns(IEnumerable<RiskPattern> patterns)
    {
        // Agrupar patrones similares y consolidar
        return patterns
            .GroupBy(p => new { p.PatternType, p.StartHour, p.EndHour })
            .Select(g => new RiskPattern(
                g.Key.PatternType,
                g.First().Description,
                g.Sum(p => p.OccurrenceCount),
                g.Average(p => p.ConfidenceScore)))
            .ToList();
    }

    private string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Sunday => "domingos",
            DayOfWeek.Monday => "lunes",
            DayOfWeek.Tuesday => "martes",
            DayOfWeek.Wednesday => "miércoles",
            DayOfWeek.Thursday => "jueves",
            DayOfWeek.Friday => "viernes",
            DayOfWeek.Saturday => "sábados",
            _ => day.ToString()
        };
    }
}
