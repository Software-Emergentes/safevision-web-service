namespace SafeVisionPlatform.Management.Domain.Model.Entities;

/// <summary>
/// Representa un evento crítico que requiere gestión por parte de los gerentes.
/// </summary>
public class CriticalEvent
{
    public int Id { get; private set; }

    /// <summary>
    /// Tipo de evento crítico.
    /// </summary>
    public CriticalEventType EventType { get; private set; }

    /// <summary>
    /// ID del conductor involucrado.
    /// </summary>
    public int DriverId { get; private set; }

    /// <summary>
    /// ID del viaje asociado.
    /// </summary>
    public int? TripId { get; private set; }

    /// <summary>
    /// ID del gerente que gestiona el evento.
    /// </summary>
    public int? ManagedByManagerId { get; private set; }

    /// <summary>
    /// Descripción del evento.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Severidad del evento.
    /// </summary>
    public string Severity { get; private set; } = string.Empty;

    /// <summary>
    /// Ubicación del evento (si aplica).
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// Estado del evento.
    /// </summary>
    public CriticalEventStatus Status { get; private set; }

    /// <summary>
    /// Acciones tomadas.
    /// </summary>
    public List<string> ActionsTaken { get; private set; } = new();

    /// <summary>
    /// Referencia de seguro (si aplica).
    /// </summary>
    public string? InsuranceReference { get; private set; }

    /// <summary>
    /// Indica si se despachó respuesta de emergencia.
    /// </summary>
    public bool EmergencyResponseDispatched { get; private set; }

    /// <summary>
    /// Fecha de ocurrencia.
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Fecha de resolución.
    /// </summary>
    public DateTime? ResolvedAt { get; private set; }

    /// <summary>
    /// Notas adicionales.
    /// </summary>
    public string? Notes { get; private set; }

    private CriticalEvent() { }

    public CriticalEvent(
        CriticalEventType eventType,
        int driverId,
        string description,
        string severity,
        int? tripId = null,
        string? location = null)
    {
        EventType = eventType;
        DriverId = driverId;
        Description = description;
        Severity = severity;
        TripId = tripId;
        Location = location;
        Status = CriticalEventStatus.Reported;
        OccurredAt = DateTime.UtcNow;
    }

    public void AssignManager(int managerId)
    {
        ManagedByManagerId = managerId;
        Status = CriticalEventStatus.InProgress;
    }

    public void AddAction(string action)
    {
        ActionsTaken.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {action}");
    }

    public void DispatchEmergencyResponse()
    {
        EmergencyResponseDispatched = true;
        AddAction("Equipo de respuesta de emergencia despachado");
    }

    public void NotifyInsurance(string reference)
    {
        InsuranceReference = reference;
        AddAction($"Aseguradora notificada. Ref: {reference}");
    }

    public void Resolve(string notes)
    {
        Status = CriticalEventStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        Notes = notes;
        AddAction($"Evento resuelto: {notes}");
    }

    public void Close()
    {
        Status = CriticalEventStatus.Closed;
    }
}

/// <summary>
/// Tipos de eventos críticos.
/// </summary>
public enum CriticalEventType
{
    Accident = 1,               // Accidente
    MicroSleep = 2,             // Micro-sueño detectado
    SevereFatigue = 3,          // Fatiga severa
    MedicalEmergency = 4,       // Emergencia médica
    VehicleBreakdown = 5,       // Avería de vehículo
    SecurityIncident = 6        // Incidente de seguridad
}

/// <summary>
/// Estados de un evento crítico.
/// </summary>
public enum CriticalEventStatus
{
    Reported = 1,       // Reportado
    InProgress = 2,     // En progreso
    Resolved = 3,       // Resuelto
    Closed = 4          // Cerrado
}
