using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeVisionPlatform.Driver.Application.CommandServices;
using SafeVisionPlatform.Driver.Application.OutBoundService;
using SafeVisionPlatform.Driver.Application.QueryServices;
using SafeVisionPlatform.Driver.Domain.Repositories;
using SafeVisionPlatform.Driver.Domain.Services;
using SafeVisionPlatform.Driver.Infrastructure.Persistence.EFC;
using SafeVisionPlatform.Driver.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Driver.Interfaces.ACL;
using SafeVisionPlatform.Driver.Interfaces.ACL.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Driver.Infrastructure.Configuration;

/// <summary>
/// Extensiones de configuración para inyección de dependencias del Bounded Context Driver.
/// </summary>
public static class DriverServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios del Bounded Context Driver.
    /// </summary>
    public static IServiceCollection AddDriverBoundedContext(this IServiceCollection services)
    {
        // ✅ IMPORTANTE: Necesitamos acceso a IConfiguration para el DbContext
        // Por ahora, registrar solo los servicios sin DbContext
        // El DbContext se debe registrar en Program.cs directamente
        
        // ✅ Registrar Repositorios
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IDriverLicenseRepository, DriverLicenseRepository>();
        
        // ✅ Registrar Servicios de Dominio
        services.AddScoped<IDriverRegistrationService, DriverRegistrationService>();
        services.AddScoped<IDriverLicenseValidationService, DriverLicenseValidationService>();
        services.AddScoped<IDriverAvailabilityService, DriverAvailabilityService>();
        
        // ✅ Registrar Servicios de Aplicación
        services.AddScoped<IDriverCommandService, DriverCommandService>();
        services.AddScoped<IDriverQueryService, DriverQueryService>();
        
        // ✅ Registrar Fachada ACL
        services.AddScoped<IDriverContextFacade, DriverContextFacade>();
        
        return services;
    }
}