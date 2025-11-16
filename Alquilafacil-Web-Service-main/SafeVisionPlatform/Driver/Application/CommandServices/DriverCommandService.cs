using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Application.Assemblers;
using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;
using SafeVisionPlatform.Driver.Domain.Repositories;
using SafeVisionPlatform.Driver.Domain.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Driver.Application.CommandServices;

/// <summary>
/// Implementación del servicio de aplicación que gestiona comandos de conductor.
/// </summary>
public class DriverCommandService : IDriverCommandService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverLicenseRepository _licenseRepository;
    private readonly IDriverRegistrationService _registrationService;
    private readonly IDriverLicenseValidationService _licenseValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public DriverCommandService(
        IDriverRepository driverRepository,
        IDriverLicenseRepository licenseRepository,
        IDriverRegistrationService registrationService,
        IDriverLicenseValidationService licenseValidationService,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _licenseRepository = licenseRepository;
        _registrationService = registrationService;
        _licenseValidationService = licenseValidationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<DriverDTO> RegisterDriverAsync(DriverRegistrationDTO registrationDto)
    {
        var driver = await _registrationService.RegisterDriverAsync(
            registrationDto.UserId,
            registrationDto.Username,
            registrationDto.EncryptedPassword,
            registrationDto.FirstName,
            registrationDto.LastName,
            registrationDto.PhoneNumber,
            registrationDto.Email,
            registrationDto.Address,
            registrationDto.YearsExperience,
            registrationDto.LicenseNumber,
            registrationDto.LicenseCategory,
            registrationDto.LicenseIssuedDate,
            registrationDto.LicenseExpirationDate
        );

        await _driverRepository.AddAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<DriverDTO> UpdateProfileAsync(int driverId, DriverProfileDTO profileDto)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        driver.UpdateProfile(
            profileDto.FirstName,
            profileDto.LastName,
            profileDto.PhoneNumber,
            profileDto.Email,
            profileDto.Address,
            profileDto.YearsExperience
        );

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<DriverLicenseDTO> UpdateLicenseAsync(int driverId, DriverLicenseDTO licenseDto)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        var licenseNumber = LicenseNumber.Create(licenseDto.LicenseNumber);
        driver.UpdateLicense(
            licenseNumber,
            licenseDto.Category,
            licenseDto.IssuedDate,
            licenseDto.ExpirationDate
        );

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverLicenseResourceFromEntityAssembler.ToResourceFromEntity(driver.License);
    }

    public async Task<DriverDTO> ChangeStatusAsync(int driverId, int statusValue)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        var newStatusEnum = DriverStatus.FromValue(statusValue);
        driver.ChangeStatus(newStatusEnum.Value);

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<DriverLicenseDTO> ValidateLicenseAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        var isValid = await _licenseValidationService.ValidateLicenseAsync(driver.License);
        if (!isValid)
            throw new InvalidOperationException("License validation failed");

        driver.ValidateLicense();

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverLicenseResourceFromEntityAssembler.ToResourceFromEntity(driver.License);
    }

    public async Task<DriverDTO> ActivateDriverAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        driver.Activate();

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<DriverDTO> DeactivateDriverAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        driver.Deactivate();

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }

    public async Task<DriverDTO> SuspendDriverAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver is null)
            throw new InvalidOperationException($"Driver with ID {driverId} not found");

        driver.Suspend();

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CompleteAsync();

        return DriverResourceFromEntityAssembler.ToResourceFromEntity(driver);
    }
}
