using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.Commands;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.CommandServices;

/// <summary>
/// Procesa el comando de inicio de viaje proveniente de la capa de interfaz
/// y ejecuta la lógica de creación del viaje en el dominio.
/// </summary>
public interface IStartTripCommandHandler
{
    Task<TripAggregate> Handle(StartTripCommand command);
}

public class StartTripCommandHandler : IStartTripCommandHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripManagerService _tripManagerService;
    private readonly IUnitOfWork _unitOfWork;

    public StartTripCommandHandler(ITripRepository tripRepository, ITripManagerService tripManagerService, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _tripManagerService = tripManagerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TripAggregate> Handle(StartTripCommand command)
    {
        // Validar disponibilidad del conductor y vehículo
        var isAvailable = await _tripManagerService.ValidateDriverAndVehicleAvailabilityAsync(command.DriverId, command.VehicleId);
        if (!isAvailable)
            throw new InvalidOperationException("El conductor o vehículo no están disponibles.");

        // Crear la nueva entidad Trip
        var trip = new TripAggregate(command.DriverId, command.VehicleId, command.DataPolicy);
        trip.StartTrip();

        // Persistir el viaje
        await _tripRepository.AddAsync(trip);
        await _unitOfWork.CompleteAsync();

        return trip;
    }
}

/// <summary>
/// Ejecuta el comando que marca un viaje como finalizado y dispara el evento TripEnded.
/// </summary>
public interface IEndTripCommandHandler
{
    Task<TripAggregate> Handle(EndTripCommand command);
}

public class EndTripCommandHandler : IEndTripCommandHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripManagerService _tripManagerService;
    private readonly IUnitOfWork _unitOfWork;

    public EndTripCommandHandler(ITripRepository tripRepository, ITripManagerService tripManagerService, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _tripManagerService = tripManagerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TripAggregate> Handle(EndTripCommand command)
    {
        var trip = await _tripRepository.FindByIdAsync(command.TripId)
            ?? throw new InvalidOperationException("El viaje no existe.");

        if (!_tripManagerService.ValidateTripCanBeEnded(trip))
            throw new InvalidOperationException("El viaje no puede ser finalizado en su estado actual.");

        trip.EndTrip();

        _tripRepository.Update(trip);
        await _unitOfWork.CompleteAsync();

        return trip;
    }
}

/// <summary>
/// Procesa el comando de cancelación de viaje, validando estado actual y registrando el motivo.
/// </summary>
public interface ICancelTripCommandHandler
{
    Task<TripAggregate> Handle(CancelTripCommand command);
}

public class CancelTripCommandHandler : ICancelTripCommandHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripManagerService _tripManagerService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelTripCommandHandler(ITripRepository tripRepository, ITripManagerService tripManagerService, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _tripManagerService = tripManagerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TripAggregate> Handle(CancelTripCommand command)
    {
        var trip = await _tripRepository.FindByIdAsync(command.TripId)
            ?? throw new InvalidOperationException("El viaje no existe.");

        if (!_tripManagerService.ValidateTripCanBeCancelled(trip))
            throw new InvalidOperationException("El viaje no puede ser cancelado en su estado actual.");

        trip.CancelTrip();

        _tripRepository.Update(trip);
        await _unitOfWork.CompleteAsync();

        return trip;
    }
}

/// <summary>
/// Ejecuta la acción de sincronizar los datos del viaje con la nube,
/// validando las políticas definidas.
/// </summary>
public interface ISyncTripDataCommandHandler
{
    Task Handle(SyncTripDataCommand command);
}

public class SyncTripDataCommandHandler : ISyncTripDataCommandHandler
{
    private readonly ITripRepository _tripRepository;

    public SyncTripDataCommandHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task Handle(SyncTripDataCommand command)
    {
        var trip = await _tripRepository.FindByIdAsync(command.TripId)
            ?? throw new InvalidOperationException("El viaje no existe.");

        if (!trip.DataPolicy.SyncToCloud)
            throw new InvalidOperationException("La política de sincronización no permite enviar datos a la nube.");

        // La sincronización real se realiza en la infraestructura
        // Esta clase valida la lógica de negocio
    }
}

