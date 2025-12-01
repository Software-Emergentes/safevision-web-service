using SafeVisionPlatform.Trip.Domain.Model.Events;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.OutboundServices;

/// <summary>
/// Escucha el evento TripEnded y coordina la generación automática del reporte
/// y su notificación al gerente.
/// </summary>
public interface ITripEndedEventHandler
{
    Task Handle(TripEndedEvent tripEndedEvent);
}

public class TripEndedEventHandler : ITripEndedEventHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly IReportRepository _reportRepository;
    private readonly ITripReportGenerator _reportGenerator;

    public TripEndedEventHandler(
        ITripRepository tripRepository,
        IReportRepository reportRepository,
        ITripReportGenerator reportGenerator)
    {
        _tripRepository = tripRepository;
        _reportRepository = reportRepository;
        _reportGenerator = reportGenerator;
    }

    public async Task Handle(TripEndedEvent tripEndedEvent)
    {
        // Obtener el viaje finalizado
        var trip = await _tripRepository.FindByIdAsync(tripEndedEvent.TripId);
        if (trip == null)
            throw new InvalidOperationException("El viaje no existe.");

        // Calcular métricas del viaje
        var metrics = await _reportGenerator.CalculateMetricsAsync(trip);

        // Generar el reporte
        var report = await _reportGenerator.GenerateReportAsync(trip, metrics.DistanceKm);

        // Persistir el reporte
        await _reportRepository.AddAsync(report);
        trip.SetReport(report);
        _tripRepository.Update(trip);

        // Publicar evento de reporte generado (será manejado por Integration Layer)
    }
}

/// <summary>
/// Reacciona al evento TripDataSentToCloud, confirmando la sincronización exitosa
/// y actualizando el estado del viaje.
/// </summary>
public interface ITripDataSentEventHandler
{
    Task Handle(TripDataSentToCloudEvent tripDataSentEvent);
}

public class TripDataSentEventHandler : ITripDataSentEventHandler
{
    private readonly ITripRepository _tripRepository;

    public TripDataSentEventHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task Handle(TripDataSentToCloudEvent tripDataSentEvent)
    {
        var trip = await _tripRepository.FindByIdAsync(tripDataSentEvent.TripId);
        if (trip == null)
            throw new InvalidOperationException("El viaje no existe.");

        // Registrar en logs o auditoría que los datos fueron sincronizados
        // Actualizar metadatos del viaje si es necesario
        _tripRepository.Update(trip);
    }
}

/// <summary>
/// Maneja el evento de cancelación de viaje.
/// </summary>
public interface ITripCancelledEventHandler
{
    Task Handle(TripCancelledEvent tripCancelledEvent);
}

public class TripCancelledEventHandler : ITripCancelledEventHandler
{
    private readonly ITripRepository _tripRepository;

    public TripCancelledEventHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task Handle(TripCancelledEvent tripCancelledEvent)
    {
        var trip = await _tripRepository.FindByIdAsync(tripCancelledEvent.TripId);
        if (trip == null)
            throw new InvalidOperationException("El viaje no existe.");

        // Registrar la cancelación en auditoría
        // Notificar a otros bounded contexts si es necesario
        _tripRepository.Update(trip);
    }
}

