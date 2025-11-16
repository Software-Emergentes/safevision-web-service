using Microsoft.Extensions.DependencyInjection;
using SafeVisionPlatform.Trip.Application.Internal.CommandServices;
using SafeVisionPlatform.Trip.Application.Internal.OutboundServices;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;
using SafeVisionPlatform.Trip.Infrastructure.Domain.Services;
using SafeVisionPlatform.Trip.Infrastructure.Integration.Services;
using SafeVisionPlatform.Trip.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Trip.Interfaces.ACL;

namespace SafeVisionPlatform.Trip.Infrastructure.Configuration;

/// <summary>
/// Extensiones de configuración para inyección de dependencias del Bounded Context Trip.
/// </summary>
public static class TripServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios del Bounded Context Trip.
    /// </summary>
    public static IServiceCollection AddTripBoundedContext(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<ICriticalNotificationRepository, CriticalNotificationRepository>();
        services.AddScoped<ISecurityConfigurationRepository, SecurityConfigurationRepository>();

        // Servicios de Dominio
        services.AddScoped<ITripManagerService, TripManagerService>();
        services.AddScoped<ITripReportGenerator, TripReportGenerator>();
        services.AddScoped<IReportExportService, Application.Internal.Services.ReportExportService>();
        services.AddScoped<IFleetDashboardService, Application.Internal.Services.FleetDashboardService>();
        services.AddScoped<IDriverHistoryService, Application.Internal.Services.DriverHistoryService>();
        services.AddScoped<ICriticalNotificationService, Application.Internal.Services.CriticalNotificationService>();
        services.AddScoped<ITripRecommendationService, Application.Internal.Services.TripRecommendationService>();
        services.AddScoped<IAlertFeedbackService, Application.Internal.Services.AlertFeedbackService>();
        services.AddScoped<IDriverComparisonService, Application.Internal.Services.DriverComparisonService>();
        services.AddScoped<ISecurityConfigurationService, Application.Internal.Services.SecurityConfigurationService>();

        // Servicios de Aplicación - Command Handlers
        services.AddScoped<IStartTripCommandHandler, StartTripCommandHandler>();
        services.AddScoped<IEndTripCommandHandler, EndTripCommandHandler>();
        services.AddScoped<ICancelTripCommandHandler, CancelTripCommandHandler>();
        services.AddScoped<ISyncTripDataCommandHandler, SyncTripDataCommandHandler>();

        // Servicios de Aplicación - Query Services
        services.AddScoped<ITripQueryService, TripQueryService>();
        services.AddScoped<IReportQueryService, ReportQueryService>();
        services.AddScoped<IAlertQueryService, AlertQueryService>();

        // Servicios de Aplicación - Application Services
        services.AddScoped<ITripApplicationService, TripApplicationService>();
        services.AddScoped<ITripReportService, TripReportService>();

        // Event Handlers
        services.AddScoped<ITripEndedEventHandler, TripEndedEventHandler>();
        services.AddScoped<ITripDataSentEventHandler, TripDataSentEventHandler>();
        services.AddScoped<ITripCancelledEventHandler, TripCancelledEventHandler>();

        // Servicios de Infraestructura
        services.AddScoped<ICloudSyncService, CloudSyncService>();
        services.AddScoped<INotificationPublisher, NotificationPublisher>();

        // Fachada de Contexto
        services.AddScoped<ITripContextFacade, TripContextFacade>();

        return services;
    }
}

