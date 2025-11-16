namespace SafeVisionPlatform.Trip.Domain.Model.Entities;

/// <summary>
/// Registra la información del viaje al finalizar.
/// Puede ser generado automáticamente y enviado a diferentes actores (conductor o gerente).
/// </summary>
public class Report
{
    public int Id { get; private set; }
    public int? TripId { get; private set; }
    public int DriverId { get; private set; }
    public int VehicleId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public double DistanceKm { get; private set; }
    public int AlertCount { get; private set; }
    public string? Notes { get; private set; }
    public ReportRecipient Recipient { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    private Report() { }

    public Report(int tripId, int driverId, int vehicleId, DateTime startTime, DateTime endTime,
        double distanceKm, int alertCount, ReportRecipient recipient, string? notes = null)
    {
        TripId = tripId;
        DriverId = driverId;
        VehicleId = vehicleId;
        StartTime = startTime;
        EndTime = endTime;
        DurationMinutes = (int)(endTime - startTime).TotalMinutes;
        DistanceKm = distanceKm;
        AlertCount = alertCount;
        Notes = notes;
        Recipient = recipient;
        Status = ReportStatus.Generated;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca el reporte como enviado.
    /// </summary>
    public void MarkAsSent()
    {
        Status = ReportStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca el reporte como visto/leído.
    /// </summary>
    public void MarkAsViewed()
    {
        Status = ReportStatus.Viewed;
    }
}

/// <summary>
/// Destinatarios del reporte de viaje.
/// </summary>
public enum ReportRecipient
{
    Driver = 1,   // Conductor
    Manager = 2,  // Gerente
    Both = 3      // Ambos
}

/// <summary>
/// Estados del reporte de viaje.
/// </summary>
public enum ReportStatus
{
    Generated = 1,  // Generado
    Sent = 2,       // Enviado
    Viewed = 3,     // Visto
    Archived = 4    // Archivado
}

