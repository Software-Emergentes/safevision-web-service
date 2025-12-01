using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Model.Commands;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.CommandServices;

/// <summary>
/// Fachada principal del contexto Trip. Expone los casos de uso, gestiona transacciones
/// y coordina la comunicación entre el repositorio y los servicios de dominio.
/// </summary>
public interface ITripApplicationService
{
    Task<TripDTO> StartTripAsync(CreateTripDTO createTripDTO);
    Task<TripDTO> EndTripAsync(int tripId);
    Task<TripDTO> CancelTripAsync(int tripId, string? reason = null);
    Task SyncTripDataAsync(int tripId);
    Task AddAlertAsync(CreateAlertDTO createAlertDTO);
    Task<TripReportDTO> GenerateReportAsync(GenerateReportDTO generateReportDTO);
}

public class TripApplicationService : ITripApplicationService
{
    private readonly IStartTripCommandHandler _startTripCommandHandler;
    private readonly IEndTripCommandHandler _endTripCommandHandler;
    private readonly ICancelTripCommandHandler _cancelTripCommandHandler;
    private readonly ISyncTripDataCommandHandler _syncTripDataCommandHandler;
    private readonly ITripReportGenerator _reportGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public TripApplicationService(
        IStartTripCommandHandler startTripCommandHandler,
        IEndTripCommandHandler endTripCommandHandler,
        ICancelTripCommandHandler cancelTripCommandHandler,
        ISyncTripDataCommandHandler syncTripDataCommandHandler,
        ITripReportGenerator reportGenerator,
        IUnitOfWork unitOfWork)
    {
        _startTripCommandHandler = startTripCommandHandler;
        _endTripCommandHandler = endTripCommandHandler;
        _cancelTripCommandHandler = cancelTripCommandHandler;
        _syncTripDataCommandHandler = syncTripDataCommandHandler;
        _reportGenerator = reportGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<TripDTO> StartTripAsync(CreateTripDTO createTripDTO)
    {
        var command = new StartTripCommand(
            createTripDTO.DriverId,
            createTripDTO.VehicleId,
            new Domain.Model.ValueObjects.TripDataPolicy(
                syncToCloud: createTripDTO.SyncToCloud,
                syncIntervalMinutes: createTripDTO.SyncIntervalMinutes
            )
        );

        var trip = await _startTripCommandHandler.Handle(command);
        return MapToDTO(trip);
    }

    public async Task<TripDTO> EndTripAsync(int tripId)
    {
        var command = new EndTripCommand(tripId);
        var trip = await _endTripCommandHandler.Handle(command);
        return MapToDTO(trip);
    }

    public async Task<TripDTO> CancelTripAsync(int tripId, string? reason = null)
    {
        var command = new CancelTripCommand(tripId, reason);
        var trip = await _cancelTripCommandHandler.Handle(command);
        return MapToDTO(trip);
    }

    public async Task SyncTripDataAsync(int tripId)
    {
        var command = new SyncTripDataCommand(tripId);
        await _syncTripDataCommandHandler.Handle(command);
    }

    public async Task AddAlertAsync(CreateAlertDTO createAlertDTO)
    {
        // Implementación en la infraestructura
        await Task.CompletedTask;
    }

    public async Task<TripReportDTO> GenerateReportAsync(GenerateReportDTO generateReportDTO)
    {
        // Implementación en la infraestructura
        return await Task.FromResult(new TripReportDTO());
    }

    private TripDTO MapToDTO(dynamic trip)
    {
        return new TripDTO
        {
            Id = trip.Id,
            DriverId = trip.DriverId,
            VehicleId = trip.VehicleId,
            Status = (int)trip.Status,
            StartTime = trip.Time.StartTime,
            EndTime = trip.Time.EndTime,
            DurationMinutes = trip.GetDurationInMinutes(),
            AlertCount = trip.Alerts.Count,
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt
        };
    }
}

/// <summary>
/// Gestiona la creación y envío de reportes de viaje.
/// </summary>
public interface ITripReportService
{
    Task<TripReportDTO> CreateReportAsync(GenerateReportDTO generateReportDTO);
    Task SendReportAsync(int reportId);
}

public class TripReportService : ITripReportService
{
    private readonly ITripReportGenerator _reportGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public TripReportService(ITripReportGenerator reportGenerator, IUnitOfWork unitOfWork)
    {
        _reportGenerator = reportGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<TripReportDTO> CreateReportAsync(GenerateReportDTO generateReportDTO)
    {
        // Implementación de la creación del reporte
        return await Task.FromResult(new TripReportDTO());
    }

    public async Task SendReportAsync(int reportId)
    {
        // Implementación del envío del reporte
        await Task.CompletedTask;
    }
}

