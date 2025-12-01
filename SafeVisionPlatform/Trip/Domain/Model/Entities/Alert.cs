namespace SafeVisionPlatform.Trip.Domain.Model.Entities;

/// <summary>
/// Representa las alertas de somnolencia, distracción o micro-sueño
/// detectadas durante el viaje. Está relacionada con el contexto Monitoring.
/// </summary>
public class Alert
{
    public int Id { get; private set; }
    public int TripId { get; private set; }
    public AlertType AlertType { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public double? Severity { get; private set; } // Valor entre 0 y 1
    public DateTime DetectedAt { get; private set; }
    public bool Acknowledged { get; private set; }

    // Propiedades de feedback
    public bool MarkedAsFalseAlarm { get; private set; }
    public string? FeedbackComment { get; private set; }
    public DateTime? FeedbackSubmittedAt { get; private set; }
    public int? FeedbackSubmittedBy { get; private set; } // DriverId

    private Alert() { }

    public Alert(int tripId, AlertType alertType, string description, double? severity = null)
    {
        if (severity.HasValue && (severity < 0 || severity > 1))
            throw new ArgumentException("La severidad debe estar entre 0 y 1.");

        TripId = tripId;
        AlertType = alertType;
        Description = description;
        Severity = severity;
        DetectedAt = DateTime.UtcNow;
        Acknowledged = false;
    }

    /// <summary>
    /// Marca la alerta como reconocida.
    /// </summary>
    public void Acknowledge()
    {
        Acknowledged = true;
    }

    /// <summary>
    /// Marca la alerta como falsa alarma con feedback del conductor.
    /// </summary>
    /// <param name="driverId">ID del conductor que proporciona el feedback</param>
    /// <param name="comment">Comentario opcional explicando por qué es una falsa alarma</param>
    public void MarkAsFalseAlarm(int driverId, string? comment = null)
    {
        MarkedAsFalseAlarm = true;
        FeedbackComment = comment;
        FeedbackSubmittedAt = DateTime.UtcNow;
        FeedbackSubmittedBy = driverId;
    }

    /// <summary>
    /// Agrega feedback general a la alerta sin marcarla como falsa alarma.
    /// </summary>
    /// <param name="driverId">ID del conductor que proporciona el feedback</param>
    /// <param name="comment">Comentario del conductor</param>
    public void AddFeedback(int driverId, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("El comentario no puede estar vacío", nameof(comment));

        FeedbackComment = comment;
        FeedbackSubmittedAt = DateTime.UtcNow;
        FeedbackSubmittedBy = driverId;
    }
}

/// <summary>
/// Tipos de alertas que pueden ser detectadas durante un viaje.
/// </summary>
public enum AlertType
{
    Drowsiness = 1,      // Somnolencia
    Distraction = 2,     // Distracción
    MicroSleep = 3,      // Micro-sueño
    SpeedViolation = 4,  // Violación de velocidad
    LaneDeviation = 5    // Desviación de carril
}
