using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Trip.Domain.Model.ValueObjects;

/// <summary>
/// Extensiones de utilidad para objetos de valor del dominio Trip.
/// </summary>
public static class ValueObjectExtensions
{
    /// <summary>
    /// Obtiene la descripción legible del estado del viaje.
    /// </summary>
    public static string GetDescription(this TripStatus status)
    {
        return status switch
        {
            TripStatus.Initiated => "Iniciado",
            TripStatus.InProgress => "En progreso",
            TripStatus.Completed => "Completado",
            TripStatus.Cancelled => "Cancelado",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Obtiene la descripción legible del tipo de alerta.
    /// </summary>
    public static string GetDescription(this AlertType alertType)
    {
        return alertType switch
        {
            AlertType.Drowsiness => "Somnolencia",
            AlertType.Distraction => "Distracción",
            AlertType.MicroSleep => "Micro-sueño",
            AlertType.SpeedViolation => "Violación de velocidad",
            AlertType.LaneDeviation => "Desviación de carril",
            _ => "Desconocida"
        };
    }

    /// <summary>
    /// Obtiene la descripción legible del destinatario del reporte.
    /// </summary>
    public static string GetDescription(this ReportRecipient recipient)
    {
        return recipient switch
        {
            ReportRecipient.Driver => "Conductor",
            ReportRecipient.Manager => "Gerente",
            ReportRecipient.Both => "Ambos",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Obtiene la descripción legible del estado del reporte.
    /// </summary>
    public static string GetDescription(this ReportStatus status)
    {
        return status switch
        {
            ReportStatus.Generated => "Generado",
            ReportStatus.Sent => "Enviado",
            ReportStatus.Viewed => "Visto",
            ReportStatus.Archived => "Archivado",
            _ => "Desconocido"
        };
    }
}
