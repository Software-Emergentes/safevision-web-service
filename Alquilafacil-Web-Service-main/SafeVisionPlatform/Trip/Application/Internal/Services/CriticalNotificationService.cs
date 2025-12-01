using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Model.ValueObjects;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de notificaciones críticas.
/// </summary>
public class CriticalNotificationService : ICriticalNotificationService
{
    private readonly ICriticalNotificationRepository _notificationRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CriticalNotificationService> _logger;

    public CriticalNotificationService(
        ICriticalNotificationRepository notificationRepository,
        IAlertRepository alertRepository,
        ITripRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<CriticalNotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _alertRepository = alertRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<NotificationResponseDTO> CreateAndSendCriticalNotificationAsync(CreateCriticalNotificationDTO notificationDto)
    {
        try
        {
            var notification = new CriticalNotification(
                notificationDto.DriverId,
                notificationDto.TripId,
                notificationDto.ManagerId,
                notificationDto.Severity,
                notificationDto.AlertType,
                notificationDto.CriticalAlertsCount,
                notificationDto.Message,
                notificationDto.Channel
            );

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            // Simular envío de notificación
            notification.MarkAsSent();
            await _notificationRepository.UpdateAsync(notification);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Notificación crítica enviada: Driver {notificationDto.DriverId}, Trip {notificationDto.TripId}");

            return new NotificationResponseDTO
            {
                NotificationId = notification.Id,
                Success = true,
                Message = "Notificación crítica enviada exitosamente",
                SentAt = notification.SentAt ?? DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar notificación crítica: {ex.Message}");
            return new NotificationResponseDTO
            {
                NotificationId = 0,
                Success = false,
                Message = $"Error al enviar notificación: {ex.Message}",
                SentAt = DateTime.UtcNow
            };
        }
    }

    public async Task<IEnumerable<CriticalNotificationDTO>> GetManagerNotificationsAsync(int managerId)
    {
        var notifications = await _notificationRepository.GetNotificationsByManagerIdAsync(managerId);
        return notifications.Select(ToDTO);
    }

    public async Task<IEnumerable<CriticalNotificationDTO>> GetPendingManagerNotificationsAsync(int managerId)
    {
        var notifications = await _notificationRepository.GetPendingNotificationsByManagerIdAsync(managerId);
        return notifications.Select(ToDTO);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _notificationRepository.FindByIdAsync(notificationId);
        if (notification == null)
            throw new InvalidOperationException($"Notification with ID {notificationId} not found");

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Notificación {notificationId} marcada como leída");
    }

    public async Task MarkAsAcknowledgedAsync(int notificationId)
    {
        var notification = await _notificationRepository.FindByIdAsync(notificationId);
        if (notification == null)
            throw new InvalidOperationException($"Notification with ID {notificationId} not found");

        notification.MarkAsAcknowledged();
        await _notificationRepository.UpdateAsync(notification);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Notificación {notificationId} marcada como reconocida");
    }

    public async Task EvaluateAndNotifyIfCriticalAsync(int driverId, int tripId)
    {
        // Obtener el viaje
        var trip = await _tripRepository.FindByIdAsync(tripId);
        if (trip == null || trip.Status != TripStatus.InProgress)
            return;

        // Obtener alertas del viaje actual
        var alerts = await _alertRepository.GetAlertsByTripIdAsync(tripId);
        var criticalAlerts = alerts.Where(a => IsCriticalAlertType((int)a.AlertType)).ToList();

        // Si hay 3 o más alertas críticas, enviar notificación
        if (criticalAlerts.Count() >= 3)
        {
            // Verificar si ya se envió notificación recientemente (últimos 10 minutos)
            var recentNotifications = await _notificationRepository.GetNotificationsByTripIdAsync(tripId);
            var hasRecentNotification = recentNotifications.Any(n =>
                n.Timestamp > DateTime.UtcNow.AddMinutes(-10) &&
                n.Severity == "Critical"
            );

            if (!hasRecentNotification)
            {
                var severity = DetermineSeverity(criticalAlerts.Count());
                var message = $"ALERTA CRÍTICA: Conductor #{driverId} presenta {criticalAlerts.Count()} alertas críticas de fatiga. Se requiere intervención inmediata.";

                var notificationDto = new CreateCriticalNotificationDTO
                {
                    DriverId = driverId,
                    TripId = tripId,
                    ManagerId = null, // Se envía a todos los gerentes
                    Severity = severity,
                    AlertType = (int)criticalAlerts.Last().AlertType,
                    CriticalAlertsCount = criticalAlerts.Count(),
                    Message = message,
                    Channel = "InApp"
                };

                await CreateAndSendCriticalNotificationAsync(notificationDto);
            }
        }
    }

    public async Task SendSafeTripCompletedNotificationAsync(int driverId, int tripId, int? managerId = null)
    {
        // Obtener el viaje
        var trip = await _tripRepository.FindByIdAsync(tripId);
        if (trip == null)
            return;

        // Solo enviar si el viaje está completado
        if (trip.Status != TripStatus.Completed)
            return;

        // Obtener alertas del viaje
        var alerts = await _alertRepository.GetAlertsByTripIdAsync(tripId);
        var criticalAlerts = alerts.Where(a => IsCriticalAlertType((int)a.AlertType)).ToList();

        // Solo enviar si NO hay alertas críticas
        if (!criticalAlerts.Any())
        {
            var duration = trip.Time.EndTime.HasValue
                ? (int)(trip.Time.EndTime.Value - trip.Time.StartTime).TotalMinutes
                : 0;

            var distance = trip.DataPolicy.TotalDistanceKm;

            var message = $"VIAJE SEGURO COMPLETADO: Conductor #{driverId} completó exitosamente el viaje #{tripId} " +
                         $"sin alertas críticas. Duración: {duration} minutos, Distancia: {distance:F2} km.";

            var notificationDto = new CreateCriticalNotificationDTO
            {
                DriverId = driverId,
                TripId = tripId,
                ManagerId = managerId,
                Severity = "Low", // Viaje seguro es baja prioridad pero importante registrar
                AlertType = 0, // Tipo general
                CriticalAlertsCount = 0,
                Message = message,
                Channel = "InApp"
            };

            await CreateAndSendCriticalNotificationAsync(notificationDto);

            _logger.LogInformation($"Notificación de viaje seguro enviada para viaje {tripId}");
        }
    }

    private bool IsCriticalAlertType(int alertType)
    {
        // 0=Drowsiness, 3=MicroSleep son críticos
        return alertType == 0 || alertType == 3;
    }

    private string DetermineSeverity(int criticalAlertsCount)
    {
        return criticalAlertsCount switch
        {
            >= 5 => "Critical",
            >= 3 => "High",
            >= 2 => "Medium",
            _ => "Low"
        };
    }

    private CriticalNotificationDTO ToDTO(CriticalNotification notification)
    {
        return new CriticalNotificationDTO
        {
            Id = notification.Id,
            DriverId = notification.DriverId,
            DriverName = $"Conductor #{notification.DriverId}",
            TripId = notification.TripId,
            ManagerId = notification.ManagerId,
            Severity = notification.Severity,
            AlertType = notification.AlertType,
            CriticalAlertsCount = notification.CriticalAlertsCount,
            Message = notification.Message,
            Timestamp = notification.Timestamp,
            Status = notification.Status,
            Channel = notification.Channel
        };
    }
}
