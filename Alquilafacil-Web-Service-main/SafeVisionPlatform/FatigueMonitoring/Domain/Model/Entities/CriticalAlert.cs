using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;

/// <summary>
/// Registra una alerta crítica generada por fatiga severa durante una sesión de conducción.
/// </summary>
public class CriticalAlert
{
    public int Id { get; private set; }

    /// <summary>
    /// ID del conductor asociado a la alerta.
    /// </summary>
    public int DriverId { get; private set; }

    /// <summary>
    /// ID del viaje durante el cual se generó la alerta.
    /// </summary>
    public int TripId { get; private set; }

    /// <summary>
    /// ID del gerente responsable de la flota.
    /// </summary>
    public int? ManagerId { get; private set; }

    /// <summary>
    /// Tipo de alerta crítica.
    /// </summary>
    public CriticalAlertType AlertType { get; private set; }

    /// <summary>
    /// Puntuación de severidad de la alerta.
    /// </summary>
    public SeverityScore Severity { get; private set; } = null!;

    /// <summary>
    /// Valor de severidad para persistencia.
    /// </summary>
    public double SeverityValue { get; private set; }

    /// <summary>
    /// Nivel de severidad para persistencia.
    /// </summary>
    public string SeverityLevel { get; private set; } = string.Empty;

    /// <summary>
    /// Mensaje descriptivo de la alerta.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Estado actual de la alerta.
    /// </summary>
    public CriticalAlertStatus Status { get; private set; }

    /// <summary>
    /// Canal de notificación utilizado.
    /// </summary>
    public string NotificationChannel { get; private set; } = string.Empty;

    /// <summary>
    /// Cantidad de eventos de somnolencia que originaron la alerta.
    /// </summary>
    public int DrowsinessEventsCount { get; private set; }

    /// <summary>
    /// Timestamp de generación de la alerta.
    /// </summary>
    public DateTime GeneratedAt { get; private set; }

    /// <summary>
    /// Timestamp de envío de la notificación.
    /// </summary>
    public DateTime? SentAt { get; private set; }

    /// <summary>
    /// Timestamp de reconocimiento de la alerta.
    /// </summary>
    public DateTime? AcknowledgedAt { get; private set; }

    /// <summary>
    /// ID del usuario que reconoció la alerta.
    /// </summary>
    public int? AcknowledgedBy { get; private set; }

    /// <summary>
    /// Acción tomada en respuesta a la alerta.
    /// </summary>
    public string? ActionTaken { get; private set; }

    private CriticalAlert() { }

    public CriticalAlert(
        int driverId,
        int tripId,
        int? managerId,
        CriticalAlertType alertType,
        SeverityScore severity,
        string message,
        int drowsinessEventsCount,
        string notificationChannel = "InApp")
    {
        DriverId = driverId;
        TripId = tripId;
        ManagerId = managerId;
        AlertType = alertType;
        Severity = severity;
        SeverityValue = severity.Value;
        SeverityLevel = severity.Level;
        Message = message;
        DrowsinessEventsCount = drowsinessEventsCount;
        NotificationChannel = notificationChannel;
        Status = CriticalAlertStatus.Pending;
        GeneratedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca la alerta como enviada.
    /// </summary>
    public void MarkAsSent()
    {
        Status = CriticalAlertStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca la alerta como reconocida.
    /// </summary>
    public void Acknowledge(int userId, string? actionTaken = null)
    {
        Status = CriticalAlertStatus.Acknowledged;
        AcknowledgedAt = DateTime.UtcNow;
        AcknowledgedBy = userId;
        ActionTaken = actionTaken;
    }

    /// <summary>
    /// Marca la alerta como resuelta.
    /// </summary>
    public void Resolve(string actionTaken)
    {
        Status = CriticalAlertStatus.Resolved;
        ActionTaken = actionTaken;
    }
}

/// <summary>
/// Tipos de alertas críticas.
/// </summary>
public enum CriticalAlertType
{
    SevereDrowsiness = 1,      // Somnolencia severa
    MicroSleepDetected = 2,    // Micro-sueño detectado
    RepeatedYawning = 3,       // Bostezos repetidos
    ExtendedEyeClosure = 4,    // Cierre prolongado de ojos
    MultipleWarnings = 5       // Múltiples advertencias acumuladas
}

/// <summary>
/// Estados de una alerta crítica.
/// </summary>
public enum CriticalAlertStatus
{
    Pending = 1,        // Pendiente de envío
    Sent = 2,           // Enviada
    Acknowledged = 3,   // Reconocida
    Resolved = 4        // Resuelta
}
