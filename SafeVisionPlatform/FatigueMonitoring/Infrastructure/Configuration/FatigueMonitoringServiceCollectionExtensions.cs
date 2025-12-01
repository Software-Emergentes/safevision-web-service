using Microsoft.Extensions.DependencyInjection;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.CommandServices;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.QueryServices;
using SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;
using SafeVisionPlatform.FatigueMonitoring.Domain.Services;
using SafeVisionPlatform.FatigueMonitoring.Infrastructure.Integration;
using SafeVisionPlatform.FatigueMonitoring.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.FatigueMonitoring.Infrastructure.Services;

namespace SafeVisionPlatform.FatigueMonitoring.Infrastructure.Configuration;

/// <summary>
/// Extensiones de configuración para inyección de dependencias del Bounded Context Fatigue Monitoring.
/// </summary>
public static class FatigueMonitoringServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios del Bounded Context Fatigue Monitoring.
    /// </summary>
    public static IServiceCollection AddFatigueMonitoringBoundedContext(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<IDrowsinessEventRepository, DrowsinessEventRepository>();
        services.AddScoped<ICriticalAlertRepository, FatigueCriticalAlertRepository>();

        // Servicios de Dominio
        services.AddScoped<IFatigueDetectionService, FatigueDetectionServiceImpl>();
        services.AddScoped<IAlertGenerationService, AlertGenerationServiceImpl>();

        // Servicios de Aplicación - Command Services
        services.AddScoped<FatigueDetectionCommandService>();

        // Servicios de Aplicación - Query Services
        services.AddScoped<FatigueQueryService>();

        // Servicios de Infraestructura
        services.AddScoped<NotificationAdapter>();

        return services;
    }
}
