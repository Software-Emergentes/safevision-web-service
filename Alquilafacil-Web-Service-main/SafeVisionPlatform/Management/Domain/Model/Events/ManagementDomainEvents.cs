namespace SafeVisionPlatform.Management.Domain.Model.Events;

/// <summary>
/// Evento que indica que un gerente ha reasignado a un conductor en un viaje en curso.
/// </summary>
public class ManagerReassignedDriverEvent
{
    public int ManagerId { get; }
    public int OriginalDriverId { get; }
    public int NewDriverId { get; }
    public int TripId { get; }
    public string Reason { get; }
    public DateTime OccurredAt { get; }

    public ManagerReassignedDriverEvent(
        int managerId,
        int originalDriverId,
        int newDriverId,
        int tripId,
        string reason)
    {
        ManagerId = managerId;
        OriginalDriverId = originalDriverId;
        NewDriverId = newDriverId;
        TripId = tripId;
        Reason = reason;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que indica que una intervención temprana evitó un accidente.
/// </summary>
public class AccidentPreventedEvent
{
    public int DriverId { get; }
    public int TripId { get; }
    public int? ManagerId { get; }
    public string InterventionType { get; }
    public string Description { get; }
    public DateTime OccurredAt { get; }

    public AccidentPreventedEvent(
        int driverId,
        int tripId,
        int? managerId,
        string interventionType,
        string description)
    {
        DriverId = driverId;
        TripId = tripId;
        ManagerId = managerId;
        InterventionType = interventionType;
        Description = description;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que refleja la asignación de un conductor de respaldo.
/// </summary>
public class BackupDriverAssignedEvent
{
    public int OriginalDriverId { get; }
    public int BackupDriverId { get; }
    public int TripId { get; }
    public int AssignedByManagerId { get; }
    public string Reason { get; }
    public DateTime OccurredAt { get; }

    public BackupDriverAssignedEvent(
        int originalDriverId,
        int backupDriverId,
        int tripId,
        int assignedByManagerId,
        string reason)
    {
        OriginalDriverId = originalDriverId;
        BackupDriverId = backupDriverId;
        TripId = tripId;
        AssignedByManagerId = assignedByManagerId;
        Reason = reason;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que indica que un accidente ocurrió durante el viaje.
/// </summary>
public class AccidentOccurredEvent
{
    public int DriverId { get; }
    public int TripId { get; }
    public string AccidentType { get; }
    public string Severity { get; }
    public string? Location { get; }
    public DateTime OccurredAt { get; }

    public AccidentOccurredEvent(
        int driverId,
        int tripId,
        string accidentType,
        string severity,
        string? location = null)
    {
        DriverId = driverId;
        TripId = tripId;
        AccidentType = accidentType;
        Severity = severity;
        Location = location;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que refleja que el equipo de respuesta de emergencias ha sido enviado.
/// </summary>
public class EmergencyResponseDispatchedEvent
{
    public int TripId { get; }
    public int DriverId { get; }
    public string ResponseTeamId { get; }
    public string IncidentType { get; }
    public string? Location { get; }
    public DateTime DispatchedAt { get; }

    public EmergencyResponseDispatchedEvent(
        int tripId,
        int driverId,
        string responseTeamId,
        string incidentType,
        string? location = null)
    {
        TripId = tripId;
        DriverId = driverId;
        ResponseTeamId = responseTeamId;
        IncidentType = incidentType;
        Location = location;
        DispatchedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que confirma la notificación a la aseguradora.
/// </summary>
public class InsuranceNotifiedEvent
{
    public int TripId { get; }
    public int DriverId { get; }
    public string IncidentType { get; }
    public string InsuranceReference { get; }
    public DateTime NotifiedAt { get; }

    public InsuranceNotifiedEvent(
        int tripId,
        int driverId,
        string incidentType,
        string insuranceReference)
    {
        TripId = tripId;
        DriverId = driverId;
        IncidentType = incidentType;
        InsuranceReference = insuranceReference;
        NotifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que refleja la suspensión temporal de un conductor.
/// </summary>
public class DriverSuspensionInitiatedEvent
{
    public int DriverId { get; }
    public int InitiatedByManagerId { get; }
    public string Reason { get; }
    public DateTime SuspendedUntil { get; }
    public DateTime InitiatedAt { get; }

    public DriverSuspensionInitiatedEvent(
        int driverId,
        int initiatedByManagerId,
        string reason,
        DateTime suspendedUntil)
    {
        DriverId = driverId;
        InitiatedByManagerId = initiatedByManagerId;
        Reason = reason;
        SuspendedUntil = suspendedUntil;
        InitiatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento que refleja que la suspensión del conductor ha sido levantada.
/// </summary>
public class DriverSuspensionLiftedEvent
{
    public int DriverId { get; }
    public int LiftedByManagerId { get; }
    public string? Notes { get; }
    public DateTime LiftedAt { get; }

    public DriverSuspensionLiftedEvent(
        int driverId,
        int liftedByManagerId,
        string? notes = null)
    {
        DriverId = driverId;
        LiftedByManagerId = liftedByManagerId;
        Notes = notes;
        LiftedAt = DateTime.UtcNow;
    }
}
