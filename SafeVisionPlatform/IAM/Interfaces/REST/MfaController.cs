using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;
using SafeVisionPlatform.IAM.Interfaces.REST.Transform;

namespace SafeVisionPlatform.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MfaController : ControllerBase
{
    private readonly IMfaCommandService _mfaCommandService;

    public MfaController(IMfaCommandService mfaCommandService)
    {
        _mfaCommandService = mfaCommandService;
    }

    /// <summary>
    /// Verifica un código MFA para un usuario
    /// </summary>
    /// <param name="request">La solicitud que contiene el userId y el código MFA</param>
    /// <returns>Token de acceso si el código MFA es válido</returns>
    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaCodeRequest request)
    {
        try
        {
            var command = VerifyMfaFromResource.ToCommandFromResource(request);
            var result = await _mfaCommandService.Handle(command);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            if (result.Token == null)
                return BadRequest(new { message = "Error al generar el token de autenticación" });

            return Ok(new 
            { 
                userId = result.User?.Id,
                username = result.User?.Username,
                token = result.Token,
                message = "Código MFA verificado correctamente"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Configura MFA para un usuario, generando una nueva clave secreta
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>La clave secreta en formato Base32 para configurar Google Authenticator</returns>
    [HttpGet("setup/{userId}")]
    [Authorize]
    public async Task<IActionResult> SetupMfa(int userId)
    {
        try
        {
            var result = await _mfaCommandService.GenerateMfaKey(userId);
            
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            // El token contiene la clave MFA en formato Base32
            string secretKey = result.Token!;
            
            // Crear una URL otpauth para el código QR
            string otpauthUrl = $"otpauth://totp/AlquilaFacil:{result.User?.Username}?secret={secretKey}&issuer=AlquilaFacil";

            return Ok(new
            {
                secretKey,
                otpauthUrl,
                message = "Clave MFA generada correctamente. Configura tu aplicación Google Authenticator con esta clave."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
