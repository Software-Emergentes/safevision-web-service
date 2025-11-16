using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Interfaces.REST.Transform;

namespace SafeVisionPlatform.Trip.Application.Internal.QueryServices;

/// <summary>
/// Define las operaciones de consulta para los viajes.
/// </summary>
public interface ITripQueryService
{
    /// <summary>
    /// Obtiene los detalles de un viaje específico.
    /// </summary>
    Task<TripDTO?> GetTripByIdAsync(int tripId);

    /// <summary>
    /// Obtiene el historial de viajes de un conductor.
    /// </summary>
    Task<IEnumerable<TripDTO>> GetTripsByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene todos los viajes de un vehículo.
    /// </summary>
    Task<IEnumerable<TripDTO>> GetTripsByVehicleIdAsync(int vehicleId);

    /// <summary>
    /// Obtiene el viaje activo de un conductor.
    /// </summary>
    Task<TripDTO?> GetActiveTripByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene viajes en un rango de fechas.
    /// </summary>
    Task<IEnumerable<TripDTO>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);
}

public class TripQueryService : ITripQueryService
{
    private readonly ITripRepository _tripRepository;

    public TripQueryService(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<TripDTO?> GetTripByIdAsync(int tripId)
    {
        var trip = await _tripRepository.FindByIdAsync(tripId);
        if (trip == null)
            return null;

        return TripAssembler.ToDTO(trip);
    }

    public async Task<IEnumerable<TripDTO>> GetTripsByDriverIdAsync(int driverId)
    {
        var trips = await _tripRepository.GetTripsByDriverIdAsync(driverId);
        return TripAssembler.ToDTOList(trips);
    }

    public async Task<IEnumerable<TripDTO>> GetTripsByVehicleIdAsync(int vehicleId)
    {
        var trips = await _tripRepository.GetTripsByVehicleIdAsync(vehicleId);
        return TripAssembler.ToDTOList(trips);
    }

    public async Task<TripDTO?> GetActiveTripByDriverIdAsync(int driverId)
    {
        var trip = await _tripRepository.GetActiveTripByDriverIdAsync(driverId);
        if (trip == null)
            return null;

        return TripAssembler.ToDTO(trip);
    }

    public async Task<IEnumerable<TripDTO>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var trips = await _tripRepository.GetTripsByDateRangeAsync(startDate, endDate);
        return TripAssembler.ToDTOList(trips);
    }
}

/// <summary>
/// Define las operaciones de consulta para reportes.
/// </summary>
public interface IReportQueryService
{
    /// <summary>
    /// Obtiene los reportes de un conductor.
    /// </summary>
    Task<IEnumerable<TripReportDTO>> GetReportsByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene todos los reportes.
    /// </summary>
    Task<IEnumerable<TripReportDTO>> GetAllReportsAsync();

    /// <summary>
    /// Obtiene el reporte de un viaje específico.
    /// </summary>
    Task<TripReportDTO?> GetReportByTripIdAsync(int tripId);

    /// <summary>
    /// Obtiene un reporte por su ID.
    /// </summary>
    Task<TripReportDTO?> GetReportByIdAsync(int reportId);
}

public class ReportQueryService : IReportQueryService
{
    private readonly IReportRepository _reportRepository;

    public ReportQueryService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<TripReportDTO>> GetReportsByDriverIdAsync(int driverId)
    {
        var reports = await _reportRepository.GetReportsByDriverIdAsync(driverId);
        return ReportAssembler.ToDTOList(reports);
    }

    public async Task<IEnumerable<TripReportDTO>> GetAllReportsAsync()
    {
        var reports = await _reportRepository.ListAsync();
        return ReportAssembler.ToDTOList(reports);
    }

    public async Task<TripReportDTO?> GetReportByTripIdAsync(int tripId)
    {
        var report = await _reportRepository.GetReportByTripIdAsync(tripId);
        if (report == null)
            return null;

        return ReportAssembler.ToDTO(report);
    }

    public async Task<TripReportDTO?> GetReportByIdAsync(int reportId)
    {
        var report = await _reportRepository.FindByIdAsync(reportId);
        if (report == null)
            return null;

        return ReportAssembler.ToDTO(report);
    }
}

/// <summary>
/// Define las operaciones de consulta para alertas.
/// </summary>
public interface IAlertQueryService
{
    /// <summary>
    /// Obtiene las alertas de un viaje específico.
    /// </summary>
    Task<IEnumerable<AlertDTO>> GetAlertsByTripIdAsync(int tripId);

    /// <summary>
    /// Obtiene alertas por tipo en un rango de fechas.
    /// </summary>
    Task<IEnumerable<AlertDTO>> GetAlertsByTypeAndDateRangeAsync(int alertType, DateTime startDate, DateTime endDate);
}

public class AlertQueryService : IAlertQueryService
{
    private readonly IAlertRepository _alertRepository;

    public AlertQueryService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<IEnumerable<AlertDTO>> GetAlertsByTripIdAsync(int tripId)
    {
        var alerts = await _alertRepository.GetAlertsByTripIdAsync(tripId);
        return AlertAssembler.ToDTOList(alerts);
    }

    public async Task<IEnumerable<AlertDTO>> GetAlertsByTypeAndDateRangeAsync(int alertType, DateTime startDate, DateTime endDate)
    {
        var alerts = await _alertRepository.GetAlertsByTypeAndDateRangeAsync(alertType, startDate, endDate);
        return AlertAssembler.ToDTOList(alerts);
    }
}
