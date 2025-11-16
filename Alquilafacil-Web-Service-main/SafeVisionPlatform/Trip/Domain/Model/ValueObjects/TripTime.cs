namespace SafeVisionPlatform.Trip.Domain.Model.ValueObjects;

/// <summary>
/// Objeto de valor que agrupa las marcas temporales del viaje.
/// Valida que la hora de inicio sea anterior a la de finalización.
/// </summary>
public class TripTime
{
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    public TripTime(DateTime startTime, DateTime? endTime)
    {
        if (endTime.HasValue && endTime <= startTime)
            throw new ArgumentException("La hora de finalización debe ser posterior a la de inicio.");

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Obtiene la duración del viaje en minutos.
    /// </summary>
    public int GetDurationInMinutes()
    {
        var end = EndTime ?? DateTime.UtcNow;
        return (int)(end - StartTime).TotalMinutes;
    }

    /// <summary>
    /// Verifica si el viaje está activo (no ha finalizado).
    /// </summary>
    public bool IsActive => EndTime == null;
}

