namespace SafeVisionPlatform.Trip.Domain.Model.Events;

/// <summary>
/// Evento que se dispara cuando un viaje es iniciado exitosamente por el conductor.
/// </summary>
public class TripStartedEvent
{
    public int TripId { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }

    public TripStartedEvent(int tripId, int driverId, int vehicleId, DateTime startTime)
    {
        TripId = tripId;
        DriverId = driverId;
        VehicleId = vehicleId;
        StartTime = startTime;
    }
}

/// <summary>
/// Evento que indica la finalización de un viaje, desencadenando la generación
/// del reporte y la sincronización de datos.
/// </summary>
public class TripEndedEvent
{
    public int TripId { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }

    public TripEndedEvent(int tripId, int driverId, int vehicleId, DateTime endTime, int durationMinutes)
    {
        TripId = tripId;
        DriverId = driverId;
        VehicleId = vehicleId;
        EndTime = endTime;
        DurationMinutes = durationMinutes;
    }
}

/// <summary>
/// Evento que comunica la cancelación de un viaje antes de su finalización.
/// </summary>
public class TripCancelledEvent
{
    public int TripId { get; set; }
    public int DriverId { get; set; }
    public string? Reason { get; set; }
    public DateTime CancelledAt { get; set; }

    public TripCancelledEvent(int tripId, int driverId, string? reason = null)
    {
        TripId = tripId;
        DriverId = driverId;
        Reason = reason;
        CancelledAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento emitido cuando los datos del viaje han sido enviados
/// y almacenados correctamente en la nube.
/// </summary>
public class TripDataSentToCloudEvent
{
    public int TripId { get; set; }
    public DateTime SyncedAt { get; set; }
    public string? SyncReference { get; set; }

    public TripDataSentToCloudEvent(int tripId, string? syncReference = null)
    {
        TripId = tripId;
        SyncedAt = DateTime.UtcNow;
        SyncReference = syncReference;
    }
}

/// <summary>
/// Evento que se dispara cuando se genera un reporte de viaje.
/// </summary>
public class TripReportGeneratedEvent
{
    public int TripId { get; set; }
    public int ReportId { get; set; }
    public int DriverId { get; set; }
    public DateTime GeneratedAt { get; set; }

    public TripReportGeneratedEvent(int tripId, int reportId, int driverId)
    {
        TripId = tripId;
        ReportId = reportId;
        DriverId = driverId;
        GeneratedAt = DateTime.UtcNow;
    }
}

