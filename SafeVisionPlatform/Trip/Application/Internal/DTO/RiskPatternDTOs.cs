namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para patrón de riesgo detectado en un conductor.
/// </summary>
public class RiskPatternDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public string PatternType { get; set; } = string.Empty; // "TimeOfDay", "DayOfWeek", "Duration", "RouteSpecific"
    public string Description { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public string Severity { get; set; } = string.Empty; // "Low", "Medium", "High", "Critical"
    public DateTime FirstDetected { get; set; }
    public DateTime LastDetected { get; set; }
    public Dictionary<string, object> PatternDetails { get; set; } = new Dictionary<string, object>();
    public List<string> Recommendations { get; set; } = new List<string>();
}

/// <summary>
/// DTO para análisis de patrones de riesgo por horario.
/// </summary>
public class TimeOfDayRiskPatternDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int HourOfDay { get; set; } // 0-23
    public string TimeRange { get; set; } = string.Empty; // "00:00-01:00"
    public int TotalAlerts { get; set; }
    public int CriticalAlerts { get; set; }
    public int TripsInThisTimeSlot { get; set; }
    public double AlertsPerTrip { get; set; }
    public double RiskScore { get; set; } // 0-100
    public string RiskLevel { get; set; } = string.Empty;
}

/// <summary>
/// DTO para análisis de patrones de riesgo por día de la semana.
/// </summary>
public class DayOfWeekRiskPatternDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int DayOfWeek { get; set; } // 0=Sunday, 6=Saturday
    public string DayName { get; set; } = string.Empty;
    public int TotalAlerts { get; set; }
    public int CriticalAlerts { get; set; }
    public int TripsOnThisDay { get; set; }
    public double AlertsPerTrip { get; set; }
    public double RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
}

/// <summary>
/// DTO para análisis de patrones de riesgo por duración de viaje.
/// </summary>
public class TripDurationRiskPatternDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int DurationRangeMinutes { get; set; } // Inicio del rango
    public string DurationRange { get; set; } = string.Empty; // "0-60 min", "60-120 min", etc.
    public int TotalTrips { get; set; }
    public int TotalAlerts { get; set; }
    public double AlertsPerTrip { get; set; }
    public double RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
}

/// <summary>
/// DTO para respuesta de análisis completo de patrones de riesgo.
/// </summary>
public class DriverRiskAnalysisDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public DateTime AnalysisStartDate { get; set; }
    public DateTime AnalysisEndDate { get; set; }
    public int TotalTripsAnalyzed { get; set; }
    public int TotalAlertsAnalyzed { get; set; }

    // Patrones detectados
    public List<RiskPatternDTO> DetectedPatterns { get; set; } = new List<RiskPatternDTO>();
    public List<TimeOfDayRiskPatternDTO> TimeOfDayPatterns { get; set; } = new List<TimeOfDayRiskPatternDTO>();
    public List<DayOfWeekRiskPatternDTO> DayOfWeekPatterns { get; set; } = new List<DayOfWeekRiskPatternDTO>();
    public List<TripDurationRiskPatternDTO> DurationPatterns { get; set; } = new List<TripDurationRiskPatternDTO>();

    // Resumen de riesgo
    public string OverallRiskLevel { get; set; } = string.Empty;
    public double OverallRiskScore { get; set; }
    public List<string> PrimaryRecommendations { get; set; } = new List<string>();

    // Horas más riesgosas
    public List<int> MostRiskyHours { get; set; } = new List<int>();
    public List<int> MostRiskyDaysOfWeek { get; set; } = new List<int>();
}

/// <summary>
/// DTO para resumen de patrones de riesgo de la flota.
/// </summary>
public class FleetRiskPatternsDTO
{
    public DateTime AnalysisStartDate { get; set; }
    public DateTime AnalysisEndDate { get; set; }
    public int TotalDriversAnalyzed { get; set; }
    public int DriversWithHighRiskPatterns { get; set; }
    public int TotalPatternsDetected { get; set; }

    public List<DriverRiskAnalysisDTO> DriverAnalyses { get; set; } = new List<DriverRiskAnalysisDTO>();

    // Patrones comunes a nivel de flota
    public List<int> FleetMostRiskyHours { get; set; } = new List<int>();
    public List<int> FleetMostRiskyDays { get; set; } = new List<int>();
    public List<RiskPatternDTO> CommonPatterns { get; set; } = new List<RiskPatternDTO>();
}

/// <summary>
/// DTO para parámetros de búsqueda de patrones.
/// </summary>
public class RiskPatternAnalysisRequestDTO
{
    public int? DriverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinimumOccurrences { get; set; } = 3; // Mínimo de ocurrencias para considerar un patrón
    public double MinimumRiskScore { get; set; } = 60.0; // Mínima puntuación de riesgo para reportar
    public bool IncludeTimeOfDayAnalysis { get; set; } = true;
    public bool IncludeDayOfWeekAnalysis { get; set; } = true;
    public bool IncludeDurationAnalysis { get; set; } = true;
}
