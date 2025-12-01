using Microsoft.Extensions.DependencyInjection;

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
        // Nota: Los servicios específicos del Driver Context serán registrados directamente 
        // en el Program.cs o en cada capa según sea necesario
        
        return services;
    }
}
