using SafeVisionPlatform.Trip.Application.Internal.CommandServices;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;

namespace SafeVisionPlatform.Trip.Interfaces.ACL;

/// <summary>
/// Fachada de contexto Trip que proporciona acceso a los servicios del dominio.
/// Se utiliza para comunicación con otros bounded contexts.
/// </summary>
public interface ITripContextFacade
{
    // Operaciones de viajes
    Task<TripDTO> StartTripAsync(CreateTripDTO createTripDTO);
    Task<TripDTO> EndTripAsync(int tripId);
    Task<TripDTO> CancelTripAsync(int tripId, string? reason = null);
    Task<TripDTO?> GetTripByIdAsync(int tripId);
    Task<IEnumerable<TripDTO>> GetTripsByDriverIdAsync(int driverId);
    Task<IEnumerable<TripDTO>> GetTripsByVehicleIdAsync(int vehicleId);
    Task<TripDTO?> GetActiveTripByDriverIdAsync(int driverId);
    Task SyncTripDataAsync(int tripId);

    // Operaciones de reportes
    Task<TripReportDTO> GenerateReportAsync(GenerateReportDTO generateReportDTO);
    Task<TripReportDTO?> GetReportByTripIdAsync(int tripId);
    Task<IEnumerable<TripReportDTO>> GetReportsByDriverIdAsync(int driverId);
    Task SendReportAsync(int reportId);

    // Operaciones de alertas
    Task<IEnumerable<AlertDTO>> GetAlertsByTripIdAsync(int tripId);
    Task AddAlertAsync(CreateAlertDTO createAlertDTO);
}

/// <summary>
/// Implementación de la fachada de contexto Trip.
/// </summary>
public class TripContextFacade : ITripContextFacade
{
    private readonly ITripApplicationService _tripApplicationService;
    private readonly ITripQueryService _tripQueryService;
    private readonly IReportQueryService _reportQueryService;
    private readonly ITripReportService _tripReportService;
    private readonly IAlertQueryService _alertQueryService;
    private readonly ILogger<TripContextFacade> _logger;

    public TripContextFacade(
        ITripApplicationService tripApplicationService,
        ITripQueryService tripQueryService,
        IReportQueryService reportQueryService,
        ITripReportService tripReportService,
        IAlertQueryService alertQueryService,
        ILogger<TripContextFacade> logger)
    {
        _tripApplicationService = tripApplicationService;
        _tripQueryService = tripQueryService;
        _reportQueryService = reportQueryService;
        _tripReportService = tripReportService;
        _alertQueryService = alertQueryService;
        _logger = logger;
    }

    // Implementación de operaciones de viajes
    public async Task<TripDTO> StartTripAsync(CreateTripDTO createTripDTO)
    {
        _logger.LogInformation($"Iniciando viaje a través de la fachada de contexto");
        return await _tripApplicationService.StartTripAsync(createTripDTO);
    }

    public async Task<TripDTO> EndTripAsync(int tripId)
    {
        _logger.LogInformation($"Finalizando viaje {tripId} a través de la fachada de contexto");
        return await _tripApplicationService.EndTripAsync(tripId);
    }

    public async Task<TripDTO> CancelTripAsync(int tripId, string? reason = null)
    {
        _logger.LogInformation($"Cancelando viaje {tripId} a través de la fachada de contexto");
        return await _tripApplicationService.CancelTripAsync(tripId, reason);
    }

    public async Task<TripDTO?> GetTripByIdAsync(int tripId)
    {
        return await _tripQueryService.GetTripByIdAsync(tripId);
    }

    public async Task<IEnumerable<TripDTO>> GetTripsByDriverIdAsync(int driverId)
    {
        return await _tripQueryService.GetTripsByDriverIdAsync(driverId);
    }

    public async Task<IEnumerable<TripDTO>> GetTripsByVehicleIdAsync(int vehicleId)
    {
        return await _tripQueryService.GetTripsByVehicleIdAsync(vehicleId);
    }

    public async Task<TripDTO?> GetActiveTripByDriverIdAsync(int driverId)
    {
        return await _tripQueryService.GetActiveTripByDriverIdAsync(driverId);
    }

    public async Task SyncTripDataAsync(int tripId)
    {
        _logger.LogInformation($"Sincronizando datos del viaje {tripId} a través de la fachada de contexto");
        await _tripApplicationService.SyncTripDataAsync(tripId);
    }

    // Implementación de operaciones de reportes
    public async Task<TripReportDTO> GenerateReportAsync(GenerateReportDTO generateReportDTO)
    {
        _logger.LogInformation($"Generando reporte para viaje {generateReportDTO.TripId} a través de la fachada de contexto");
        return await _tripReportService.CreateReportAsync(generateReportDTO);
    }

    public async Task<TripReportDTO?> GetReportByTripIdAsync(int tripId)
    {
        return await _reportQueryService.GetReportByTripIdAsync(tripId);
    }

    public async Task<IEnumerable<TripReportDTO>> GetReportsByDriverIdAsync(int driverId)
    {
        return await _reportQueryService.GetReportsByDriverIdAsync(driverId);
    }

    public async Task SendReportAsync(int reportId)
    {
        _logger.LogInformation($"Enviando reporte {reportId} a través de la fachada de contexto");
        await _tripReportService.SendReportAsync(reportId);
    }

    // Implementación de operaciones de alertas
    public async Task<IEnumerable<AlertDTO>> GetAlertsByTripIdAsync(int tripId)
    {
        return await _alertQueryService.GetAlertsByTripIdAsync(tripId);
    }

    public async Task AddAlertAsync(CreateAlertDTO createAlertDTO)
    {
        _logger.LogInformation($"Agregando alerta al viaje {createAlertDTO.TripId} a través de la fachada de contexto");
        await _tripApplicationService.AddAlertAsync(createAlertDTO);
    }
}

