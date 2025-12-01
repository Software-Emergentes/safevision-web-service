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

namespace SafeVisionPlatform.Driver.Infrastructure;

/// <summary>
/// Extensiones para registrar los servicios del contexto Driver en el contenedor de inyección de dependencias.
/// </summary>
public static class DriverServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios del contexto Driver.
    /// </summary>
    public static IServiceCollection AddDriverContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DriverDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mysqlOptions => mysqlOptions.EnableRetryOnFailure()
            )
        );

        // Registrar IUnitOfWork con el DbContext
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<DriverDbContext>());

        // Registrar Repositorios
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IDriverLicenseRepository, DriverLicenseRepository>();

        // Registrar Servicios de Dominio
        services.AddScoped<IDriverRegistrationService, DriverRegistrationService>();
        services.AddScoped<IDriverLicenseValidationService, DriverLicenseValidationService>();
        services.AddScoped<IDriverAvailabilityService, DriverAvailabilityService>();

        // Registrar Servicios de Aplicación
        services.AddScoped<IDriverCommandService, DriverCommandService>();
        services.AddScoped<IDriverQueryService, DriverQueryService>();

        // Registrar Fachada ACL
        services.AddScoped<IDriverContextFacade, DriverContextFacade>();

        return services;
    }
}
