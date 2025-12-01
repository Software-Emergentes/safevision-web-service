using SafeVisionPlatform.Management.Application.Internal.DTO;
using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Management.Domain.Repositories;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Management.Application.Internal.CommandServices;

/// <summary>
/// Servicio para gestionar eventos críticos.
/// </summary>
public class CriticalEventService
{
    private readonly ICriticalEventRepository _eventRepository;
    private readonly IManagerRepository _managerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CriticalEventService> _logger;

    public CriticalEventService(
        ICriticalEventRepository eventRepository,
        IManagerRepository managerRepository,
        IUnitOfWork unitOfWork,
        ILogger<CriticalEventService> logger)
    {
        _eventRepository = eventRepository;
        _managerRepository = managerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Crea y registra un nuevo evento crítico.
    /// </summary>
    public async Task<CriticalEventDTO> HandleCriticalEventAsync(CreateCriticalEventDTO request)
    {
        try
        {
            var eventType = Enum.Parse<CriticalEventType>(request.EventType);

            var criticalEvent = new CriticalEvent(
                eventType,
                request.DriverId,
                request.Description,
                request.Severity,
                request.TripId,
                request.Location);

            await _eventRepository.AddAsync(criticalEvent);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation(
                $"Evento crítico registrado: {criticalEvent.Id} - {eventType} para conductor {request.DriverId}");

            return MapToDTO(criticalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registrando evento crítico: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Asigna un gerente para gestionar el evento.
    /// </summary>
    public async Task AssignManagerAsync(int eventId, int managerId)
    {
        var criticalEvent = await _eventRepository.FindByIdAsync(eventId);
        if (criticalEvent == null)
            throw new InvalidOperationException($"Evento con ID {eventId} no encontrado");

        criticalEvent.AssignManager(managerId);
        await _eventRepository.UpdateAsync(criticalEvent);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Gerente {managerId} asignado al evento {eventId}");
    }

    /// <summary>
    /// Agrega una acción tomada al evento.
    /// </summary>
    public async Task AddActionAsync(int eventId, string action)
    {
        var criticalEvent = await _eventRepository.FindByIdAsync(eventId);
        if (criticalEvent == null)
            throw new InvalidOperationException($"Evento con ID {eventId} no encontrado");

        criticalEvent.AddAction(action);
        await _eventRepository.UpdateAsync(criticalEvent);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Despacha respuesta de emergencia.
    /// </summary>
    public async Task DispatchEmergencyResponseAsync(int eventId)
    {
        var criticalEvent = await _eventRepository.FindByIdAsync(eventId);
        if (criticalEvent == null)
            throw new InvalidOperationException($"Evento con ID {eventId} no encontrado");

        criticalEvent.DispatchEmergencyResponse();
        await _eventRepository.UpdateAsync(criticalEvent);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Respuesta de emergencia despachada para evento {eventId}");
    }

    /// <summary>
    /// Notifica a la aseguradora.
    /// </summary>
    public async Task NotifyInsuranceAsync(int eventId, string insuranceReference)
    {
        var criticalEvent = await _eventRepository.FindByIdAsync(eventId);
        if (criticalEvent == null)
            throw new InvalidOperationException($"Evento con ID {eventId} no encontrado");

        criticalEvent.NotifyInsurance(insuranceReference);
        await _eventRepository.UpdateAsync(criticalEvent);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Aseguradora notificada para evento {eventId}. Ref: {insuranceReference}");
    }

    /// <summary>
    /// Resuelve el evento.
    /// </summary>
    public async Task ResolveEventAsync(int eventId, string notes)
    {
        var criticalEvent = await _eventRepository.FindByIdAsync(eventId);
        if (criticalEvent == null)
            throw new InvalidOperationException($"Evento con ID {eventId} no encontrado");

        criticalEvent.Resolve(notes);
        await _eventRepository.UpdateAsync(criticalEvent);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Evento {eventId} resuelto");
    }

    /// <summary>
    /// Obtiene eventos pendientes.
    /// </summary>
    public async Task<IEnumerable<CriticalEventDTO>> GetPendingEventsAsync()
    {
        var events = await _eventRepository.GetPendingEventsAsync();
        return events.Select(MapToDTO);
    }

    /// <summary>
    /// Obtiene eventos por gerente.
    /// </summary>
    public async Task<IEnumerable<CriticalEventDTO>> GetEventsByManagerAsync(int managerId)
    {
        var events = await _eventRepository.GetByManagerIdAsync(managerId);
        return events.Select(MapToDTO);
    }

    /// <summary>
    /// Obtiene evento por ID.
    /// </summary>
    public async Task<CriticalEventDTO?> GetEventByIdAsync(int eventId)
    {
        var ev = await _eventRepository.FindByIdAsync(eventId);
        return ev != null ? MapToDTO(ev) : null;
    }

    private CriticalEventDTO MapToDTO(CriticalEvent ev)
    {
        return new CriticalEventDTO
        {
            Id = ev.Id,
            EventType = ev.EventType.ToString(),
            DriverId = ev.DriverId,
            DriverName = $"Conductor #{ev.DriverId}",
            TripId = ev.TripId,
            ManagedByManagerId = ev.ManagedByManagerId,
            Description = ev.Description,
            Severity = ev.Severity,
            Location = ev.Location,
            Status = ev.Status.ToString(),
            ActionsTaken = ev.ActionsTaken,
            InsuranceReference = ev.InsuranceReference,
            EmergencyResponseDispatched = ev.EmergencyResponseDispatched,
            OccurredAt = ev.OccurredAt,
            ResolvedAt = ev.ResolvedAt
        };
    }
}
