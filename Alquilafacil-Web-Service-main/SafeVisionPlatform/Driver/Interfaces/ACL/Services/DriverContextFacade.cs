using SafeVisionPlatform.Driver.Interfaces.ACL;
using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Application.QueryServices;

namespace SafeVisionPlatform.Driver.Interfaces.ACL.Services;

/// <summary>
/// Implementación de la fachada ACL para el contexto Driver.
/// </summary>
public class DriverContextFacade : IDriverContextFacade
{
    private readonly IDriverQueryService _queryService;

    public DriverContextFacade(IDriverQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<bool> IsDriverAvailableAsync(int driverId)
    {
        return await _queryService.IsDriverAvailableAsync(driverId);
    }

    public async Task<DriverDTO?> GetDriverBasicInfoAsync(int driverId)
    {
        return await _queryService.GetDriverByIdAsync(driverId);
    }

    public async Task<bool> IsDriverLicenseValidAsync(int driverId)
    {
        var license = await _queryService.GetDriverLicenseAsync(driverId);
        return license is not null && license.IsValidated && !license.IsExpired;
    }

    public async Task<string?> GetDriverStatusAsync(int driverId)
    {
        var driver = await _queryService.GetDriverByIdAsync(driverId);
        return driver?.Status;
    }
}
