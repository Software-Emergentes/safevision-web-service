using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Driver.Application.CommandServices;
using SafeVisionPlatform.Driver.Application.QueryServices;
using SafeVisionPlatform.Driver.Interfaces.REST.Resources;
using SafeVisionPlatform.Driver.Interfaces.REST.Transform;

namespace SafeVisionPlatform.Driver.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class DriversController : ControllerBase
{
    private readonly IDriverCommandService _commandService;
    private readonly IDriverQueryService _queryService;

    public DriversController(
        IDriverCommandService commandService,
        IDriverQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    /// <summary>
    /// Registra un nuevo conductor en el sistema.
    /// </summary>
    /// <remarks>
    /// POST /api/v1/drivers/register
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDriver([FromBody] CreateDriverResource resource)
    {
        var registrationDto = CreateDriverResourceFromAssembler.ToDto(resource);
        var driverDto = await _commandService.RegisterDriverAsync(registrationDto);
        var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);

        return CreatedAtAction(nameof(GetDriverById), new { driverId = driverResource.Id }, driverResource);
    }

    /// <summary>
    /// Obtiene los detalles completos de un conductor específico.
    /// </summary>
    /// <remarks>
    /// GET /api/v1/drivers/{driverId}
    /// </remarks>
    [HttpGet("{driverId:int}")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDriverById([FromRoute] int driverId)
    {
        var driverDto = await _queryService.GetDriverByIdAsync(driverId);
        if (driverDto is null)
            return NotFound($"Driver with ID {driverId} not found");

        var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
        return Ok(driverResource);
    }

    /// <summary>
    /// Lista todos los conductores registrados en el sistema.
    /// </summary>
    /// <remarks>
    /// GET /api/v1/drivers
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DriverResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDrivers()
    {
        var driversDto = await _queryService.GetAllDriversAsync();
        var driverResources = DriverResourceFromDtoAssembler.ToResources(driversDto);
        return Ok(driverResources);
    }

    /// <summary>
    /// Obtiene un conductor por su ID de usuario.
    /// </summary>
    /// <remarks>
    /// GET /api/v1/drivers/by-user/{userId}
    /// </remarks>
    [HttpGet("by-user/{userId:int}")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDriverByUserId([FromRoute] int userId)
    {
        var driverDto = await _queryService.GetDriverByUserIdAsync(userId);
        if (driverDto is null)
            return NotFound($"Driver with User ID {userId} not found");

        var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
        return Ok(driverResource);
    }

    /// <summary>
    /// Obtiene conductores filtrados por estado.
    /// </summary>
    /// <remarks>
    /// GET /api/v1/drivers/by-status/{statusValue}
    /// </remarks>
    [HttpGet("by-status/{statusValue:int}")]
    [ProducesResponseType(typeof(IEnumerable<DriverResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriversByStatus([FromRoute] int statusValue)
    {
        var driversDto = await _queryService.GetDriversByStatusAsync(statusValue);
        var driverResources = DriverResourceFromDtoAssembler.ToResources(driversDto);
        return Ok(driverResources);
    }

    /// <summary>
    /// Actualiza el perfil de un conductor.
    /// </summary>
    /// <remarks>
    /// PUT /api/v1/drivers/{driverId}/profile
    /// </remarks>
    [HttpPut("{driverId:int}/profile")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(
        [FromRoute] int driverId,
        [FromBody] UpdateDriverProfileResource resource)
    {
        try
        {
            var profileDto = UpdateDriverProfileResourceFromAssembler.ToDto(resource);
            var driverDto = await _commandService.UpdateProfileAsync(driverId, profileDto);
            var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
            return Ok(driverResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza o renueva la licencia de conducir de un conductor.
    /// </summary>
    /// <remarks>
    /// PUT /api/v1/drivers/{driverId}/license
    /// </remarks>
    [HttpPut("{driverId:int}/license")]
    [ProducesResponseType(typeof(DriverLicenseResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLicense(
        [FromRoute] int driverId,
        [FromBody] UpdateDriverLicenseResource resource)
    {
        try
        {
            var licenseDto = UpdateDriverLicenseResourceFromAssembler.ToDto(resource);
            var updatedLicenseDto = await _commandService.UpdateLicenseAsync(driverId, licenseDto);
            var licenseResource = DriverLicenseResourceFromDtoAssembler.ToResource(updatedLicenseDto);
            return Ok(licenseResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene la información detallada de la licencia de un conductor.
    /// </summary>
    /// <remarks>
    /// GET /api/v1/drivers/{driverId}/license
    /// </remarks>
    [HttpGet("{driverId:int}/license")]
    [ProducesResponseType(typeof(DriverLicenseResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDriverLicense([FromRoute] int driverId)
    {
        var licenseDto = await _queryService.GetDriverLicenseAsync(driverId);
        if (licenseDto is null)
            return NotFound($"License for driver with ID {driverId} not found");

        var licenseResource = DriverLicenseResourceFromDtoAssembler.ToResource(licenseDto);
        return Ok(licenseResource);
    }

    /// <summary>
    /// Valida la licencia de conducir de un conductor.
    /// </summary>
    /// <remarks>
    /// POST /api/v1/drivers/{driverId}/validate-license
    /// </remarks>
    [HttpPost("{driverId:int}/validate-license")]
    [ProducesResponseType(typeof(DriverLicenseResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateLicense([FromRoute] int driverId)
    {
        try
        {
            var licenseDto = await _commandService.ValidateLicenseAsync(driverId);
            var licenseResource = DriverLicenseResourceFromDtoAssembler.ToResource(licenseDto);
            return Ok(licenseResource);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cambia el estado del conductor.
    /// </summary>
    /// <remarks>
    /// PUT /api/v1/drivers/{driverId}/status
    /// </remarks>
    [HttpPut("{driverId:int}/status")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] int driverId,
        [FromBody] ChangeDriverStatusResource resource)
    {
        try
        {
            var driverDto = await _commandService.ChangeStatusAsync(driverId, resource.Status);
            var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
            return Ok(driverResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Activa un conductor (cambia su estado a Activo).
    /// </summary>
    /// <remarks>
    /// POST /api/v1/drivers/{driverId}/activate
    /// </remarks>
    [HttpPost("{driverId:int}/activate")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateDriver([FromRoute] int driverId)
    {
        try
        {
            var driverDto = await _commandService.ActivateDriverAsync(driverId);
            var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
            return Ok(driverResource);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Desactiva un conductor (cambia su estado a Inactivo).
    /// </summary>
    /// <remarks>
    /// POST /api/v1/drivers/{driverId}/deactivate
    /// </remarks>
    [HttpPost("{driverId:int}/deactivate")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateDriver([FromRoute] int driverId)
    {
        try
        {
            var driverDto = await _commandService.DeactivateDriverAsync(driverId);
            var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
            return Ok(driverResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Suspende un conductor.
    /// </summary>
    /// <remarks>
    /// POST /api/v1/drivers/{driverId}/suspend
    /// </remarks>
    [HttpPost("{driverId:int}/suspend")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SuspendDriver([FromRoute] int driverId)
    {
        try
        {
            var driverDto = await _commandService.SuspendDriverAsync(driverId);
            var driverResource = DriverResourceFromDtoAssembler.ToResource(driverDto);
            return Ok(driverResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Verifica la disponibilidad de un conductor para viajes.
    /// </summary>
    /// <remarks>
    /// GET /api/v1/drivers/{driverId}/availability
    /// </remarks>
    [HttpGet("{driverId:int}/availability")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckAvailability([FromRoute] int driverId)
    {
        var isAvailable = await _queryService.IsDriverAvailableAsync(driverId);
        if (!isAvailable)
            return NotFound($"Driver with ID {driverId} not found or not available");

        return Ok(new { available = isAvailable });
    }
}

