namespace SafeVisionPlatform.Management.Domain.Model.ValueObjects;

/// <summary>
/// Define un patrón recurrente de somnolencia o incidentes detectado
/// en base a datos históricos (ej. alertas en franjas horarias específicas).
/// </summary>
public class RiskPattern
{
    /// <summary>
    /// Tipo de patrón detectado.
    /// </summary>
    public RiskPatternType PatternType { get; private set; }

    /// <summary>
    /// Descripción del patrón.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Número de ocurrencias del patrón.
    /// </summary>
    public int OccurrenceCount { get; private set; }

    /// <summary>
    /// Nivel de severidad del patrón.
    /// </summary>
    public string SeverityLevel { get; private set; } = string.Empty;

    /// <summary>
    /// Puntuación de confianza del patrón (0-1).
    /// </summary>
    public double ConfidenceScore { get; private set; }

    /// <summary>
    /// Hora de inicio del patrón (si aplica).
    /// </summary>
    public int? StartHour { get; private set; }

    /// <summary>
    /// Hora de fin del patrón (si aplica).
    /// </summary>
    public int? EndHour { get; private set; }

    /// <summary>
    /// Días de la semana en que ocurre el patrón.
    /// </summary>
    public List<DayOfWeek> DaysOfWeek { get; private set; } = new();

    /// <summary>
    /// Recomendaciones basadas en el patrón.
    /// </summary>
    public List<string> Recommendations { get; private set; } = new();

    private RiskPattern() { }

    public RiskPattern(
        RiskPatternType patternType,
        string description,
        int occurrenceCount,
        double confidenceScore)
    {
        PatternType = patternType;
        Description = description;
        OccurrenceCount = occurrenceCount;
        ConfidenceScore = Math.Clamp(confidenceScore, 0, 1);
        SeverityLevel = DetermineSeverity(occurrenceCount, confidenceScore);
    }

    public void SetTimeRange(int startHour, int endHour)
    {
        StartHour = startHour;
        EndHour = endHour;
    }

    public void SetDaysOfWeek(IEnumerable<DayOfWeek> days)
    {
        DaysOfWeek = days.ToList();
    }

    public void AddRecommendation(string recommendation)
    {
        Recommendations.Add(recommendation);
    }

    private static string DetermineSeverity(int occurrences, double confidence)
    {
        var score = occurrences * confidence;
        return score switch
        {
            >= 15 => "Critical",
            >= 10 => "High",
            >= 5 => "Medium",
            _ => "Low"
        };
    }
}

/// <summary>
/// Tipos de patrones de riesgo.
/// </summary>
public enum RiskPatternType
{
    TimeOfDay = 1,          // Patrón por hora del día
    DayOfWeek = 2,          // Patrón por día de la semana
    TripDuration = 3,       // Patrón por duración de viaje
    ConsecutiveTrips = 4,   // Patrón por viajes consecutivos
    WeatherCondition = 5,   // Patrón por condiciones climáticas
    RouteSpecific = 6       // Patrón específico de ruta
}
