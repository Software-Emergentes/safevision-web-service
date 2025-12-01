using Microsoft.Extensions.DependencyInjection;
using SafeVisionPlatform.Management.Application.Internal.CommandServices;
using SafeVisionPlatform.Management.Application.Internal.QueryServices;
using SafeVisionPlatform.Management.Domain.Repositories;
using SafeVisionPlatform.Management.Domain.Services;
using SafeVisionPlatform.Management.Infrastructure.Integration;
using SafeVisionPlatform.Management.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Management.Infrastructure.Services;

namespace SafeVisionPlatform.Management.Infrastructure.Configuration;

/// <summary>
/// Extensiones de configuración para inyección de dependencias del Bounded Context Management.
/// </summary>
public static class ManagementServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios del Bounded Context Management.
    /// </summary>
    public static IServiceCollection AddManagementBoundedContext(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<IReportRepository, ReportRepositoryImpl>();
        services.AddScoped<IManagerRepository, ManagerRepositoryImpl>();
        services.AddScoped<ICriticalEventRepository, CriticalEventRepositoryImpl>();

        // Servicios de Dominio
        services.AddScoped<IRiskAnalysisService, RiskAnalysisServiceImpl>();
        services.AddScoped<IReportGeneratorService, ReportGeneratorServiceImpl>();

        // Servicios de Aplicación - Command Services
        services.AddScoped<ReportManagementService>();
        services.AddScoped<CriticalEventService>();

        // Servicios de Aplicación - Query Services
        services.AddScoped<RiskPatternQueryService>();

        // Servicios de Infraestructura
        services.AddScoped<NotificationPublisher>();

        return services;
    }
}
