namespace SafeVisionPlatform.Trip.Domain.Model.ValueObjects;

/// <summary>
/// Objeto de valor que encapsula las reglas que determinan cuándo y cómo
/// se recolectan o envían los datos del viaje hacia la nube.
/// </summary>
public class TripDataPolicy
{
    public bool SyncToCloud { get; private set; }
    public int SyncIntervalMinutes { get; private set; }
    public bool IncludeAlerts { get; private set; }
    public bool IncludeMetrics { get; private set; }

    public TripDataPolicy(bool syncToCloud = true, int syncIntervalMinutes = 5, bool includeAlerts = true, bool includeMetrics = true)
    {
        if (syncIntervalMinutes <= 0)
            throw new ArgumentException("El intervalo de sincronización debe ser mayor a 0.");

        SyncToCloud = syncToCloud;
        SyncIntervalMinutes = syncIntervalMinutes;
        IncludeAlerts = includeAlerts;
        IncludeMetrics = includeMetrics;
    }

    /// <summary>
    /// Verifica si es el momento de sincronizar basado en la hora de la última sincronización.
    /// </summary>
    public bool ShouldSyncNow(DateTime lastSyncTime)
    {
        if (!SyncToCloud)
            return false;

        return (DateTime.UtcNow - lastSyncTime).TotalMinutes >= SyncIntervalMinutes;
    }
}

