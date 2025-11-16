using SafeVisionPlatform.Driver.Application.Assemblers;
using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Domain.Repositories;
using SafeVisionPlatform.Driver.Domain.Services;

namespace SafeVisionPlatform.Driver.Application.QueryServices;

/// <summary>
/// Implementación del servicio de aplicación que gestiona consultas de conductor.
/// </summary>
public class DriverQueryService : IDriverQueryService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverAvailabilityService _availabilityService;

    public DriverQueryService(
        IDriverRepository driverRepository,
        IDriverAvailabilityService availabilityService)
    {
        _driverRepository = driverRepository;
        _availabilityService = availabilityService;
    }

    public async Task<DriverDTO?> GetDriverByIdAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        return driver is null ? null : DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<IEnumerable<DriverDTO>> GetAllDriversAsync()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return DriverResourceFromEntityAssembler.ToResourceFromEntities(drivers);
    }

    public async Task<DriverDTO?> GetDriverByUserIdAsync(int userId)
    {
        var driver = await _driverRepository.GetByUserIdAsync(userId);
        return driver is null ? null : DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<IEnumerable<DriverDTO>> GetDriversByStatusAsync(int statusValue)
    {
        var drivers = await _driverRepository.GetByStatusAsync(statusValue);
        return DriverResourceFromEntityAssembler.ToResourceFromEntities(drivers);
    }

    public async Task<DriverLicenseDTO?> GetDriverLicenseAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            return null;

        return DriverLicenseResourceFromEntityAssembler.ToResourceFromEntity(driver.License);
    }

    public async Task<DriverProfileDTO?> GetDriverProfileAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            return null;

        return DriverProfileResourceFromEntityAssembler.ToResourceFromEntity(driver.Profile);
    }

    public async Task<bool> IsDriverAvailableAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            return false;

        return _availabilityService.IsDriverAvailable(driver);
    }
}

