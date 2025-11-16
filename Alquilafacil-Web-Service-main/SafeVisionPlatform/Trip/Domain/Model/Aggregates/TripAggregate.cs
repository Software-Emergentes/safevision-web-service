using SafeVisionPlatform.Trip.Domain.Model.Commands;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Trip.Domain.Model.Aggregates;

/// <summary>
/// Representa un viaje realizado por un conductor.
/// Es la entidad raíz del agregado Trip que controla las operaciones de inicio,
/// finalización y cancelación del viaje.
/// </summary>
public class TripAggregate
{
    public int Id { get; private set; }
    public int DriverId { get; private set; }
    public int VehicleId { get; private set; }
    public TripStatus Status { get; private set; }
    public TripTime Time { get; private set; } = new TripTime(DateTime.UtcNow, null);
    public TripDataPolicy DataPolicy { get; private set; } = new TripDataPolicy(false, 5);
    public List<Alert> Alerts { get; private set; }
    public Report? Report { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private TripAggregate()
    {
        Alerts = new List<Alert>();
    }

    public TripAggregate(int driverId, int vehicleId, TripDataPolicy dataPolicy) : this()
    {
        DriverId = driverId;
        VehicleId = vehicleId;
        Status = TripStatus.Initiated;
        DataPolicy = dataPolicy;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Inicia el viaje actualizando su estado a "En progreso".
    /// </summary>
    public void StartTrip()
    {
        if (Status != TripStatus.Initiated)
            throw new InvalidOperationException("El viaje solo puede ser iniciado desde el estado Iniciado.");

        Status = TripStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Finaliza el viaje registrando la hora de fin.
    /// </summary>
    public void EndTrip()
    {
        if (Status != TripStatus.InProgress)
            throw new InvalidOperationException("Solo los viajes en progreso pueden ser finalizados.");

        Status = TripStatus.Completed;
        Time = new TripTime(Time.StartTime, DateTime.UtcNow);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancela el viaje antes de su finalización.
    /// </summary>
    public void CancelTrip()
    {
        if (Status == TripStatus.Completed || Status == TripStatus.Cancelled)
            throw new InvalidOperationException("No se puede cancelar un viaje finalizado o ya cancelado.");

        Status = TripStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Agrega una alerta al viaje.
    /// </summary>
    public void AddAlert(Alert alert)
    {
        if (Status == TripStatus.Completed || Status == TripStatus.Cancelled)
            throw new InvalidOperationException("No se pueden agregar alertas a un viaje finalizado o cancelado.");

        Alerts.Add(alert);
    }

    /// <summary>
    /// Establece el reporte del viaje.
    /// </summary>
    public void SetReport(Report report)
    {
        Report = report;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene la duración total del viaje en minutos.
    /// </summary>
    public int GetDurationInMinutes()
    {
        if (Time.EndTime == null)
            return (int)(DateTime.UtcNow - Time.StartTime).TotalMinutes;

        return (int)(Time.EndTime.Value - Time.StartTime).TotalMinutes;
    }
}
