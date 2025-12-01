using System.Net.Mime;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using SafeVisionPlatform.IAM.Application.Internal.OutboundServices;
using SafeVisionPlatform.IAM.Domain.Model.Queries;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;
using SafeVisionPlatform.IAM.Interfaces.REST.Transform;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(IUserCommandService userCommandService, IUserQueryService userQueryService, IUnitOfWork unitOfWork, ITokenService tokenService) : ControllerBase
{

    /**
     * <summary>
     *     Sign in endpoint. It allows to authenticate a user
     * </summary>
     * <param name="signInResource">The sign in resource containing username and password.</param>
     * <returns>The authenticated user resource, including a JWT token</returns>
     */
    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
    {
        try
        {
            var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
            var authenticatedUser = await userCommandService.Handle(signInCommand);
            
            // Verificar si el usuario tiene MFA configurado
            bool hasMfaKey = !string.IsNullOrEmpty(authenticatedUser.user.MfaKey);
            bool hasOneTimeCodes = authenticatedUser.user.MfaCodes != null && authenticatedUser.user.MfaCodes.Any();
            bool requiresMfa = hasMfaKey || hasOneTimeCodes;
            
            if (requiresMfa)
            {
                // Si requiere MFA, no devolver el token completo
                return Ok(new
                {
                    userId = authenticatedUser.user.Id,
                    username = authenticatedUser.user.Username,
                    email = authenticatedUser.user.Email,
                    requiresMfa = true,
                    message = "Se requiere verificación MFA para completar el inicio de sesión"
                });
            }
            
            // Si no requiere MFA, devolver respuesta normal con token
            var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                authenticatedUser.user, 
                authenticatedUser.token);
                
            return Ok(new
            {
                id = resource.Id,
                username = resource.Username,
                token = resource.Token,
                requiresMfa = false
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /**
     * <summary>
     *     Sign up endpoint. It allows to create a new user
     * </summary>
     * <param name="signUpResource">The sign up resource containing username and password.</param>
     * <returns>A confirmation message on successful creation.</returns>
     */
    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
        await userCommandService.Handle(signUpCommand);
        return Ok(new { message = "User created successfully"});
    }

    /**
     * <summary>
     *     Verify MFA code endpoint. It allows to verify the MFA code for a user and complete authentication
     * </summary>
     * <param name="verifyMfaCodeResource">The request containing user ID and MFA code.</param>
     * <returns>User token and information if MFA code is valid.</returns>
     */
    [HttpPost("verify-mfa")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaCodeRequest verifyMfaCodeResource)
    {
        try
        {
            var verifyMfaCommand = VerifyMfaFromResource.ToCommandFromResource(verifyMfaCodeResource);
            var mfaResult = await userCommandService.Handle(verifyMfaCommand);
            
            if (mfaResult != null)
            {
                // Si la verificación MFA es exitosa, buscar el usuario para generar el token
                var getUserQuery = new GetUserByIdQuery(verifyMfaCodeResource.UserId);
                var user = await userQueryService.Handle(getUserQuery);
                
                if (user != null)
                {
                    // Generar token usando el servicio de tokens (necesitamos agregar ITokenService)
                    // Por ahora, usar el método del UserCommandService para generar el token
                    var token = tokenService.GenerateToken(user); // Generar el token real
                    
                    var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(user, token);
                    
                    return Ok(new 
                    { 
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        token = token, // Aquí usamos el token generado
                        success = true,
                        message = "Autenticación MFA completada exitosamente"
                    });
                }
                
                return BadRequest(new { success = false, message = "Error al obtener información del usuario" });
            }
            else
            {
                return BadRequest(new { success = false, message = "Código MFA inválido o expirado" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
    
    /**
     * <summary>
     *     Enable MFA endpoint. Generates MFA key for a user
     * </summary>
     * <param name="request">The request containing user information</param>
     * <returns>MFA setup information including secret key and QR code URL</returns>
     */
    [HttpPost("mfa/enable")]
    [AllowAnonymous]
    public async Task<IActionResult> EnableMfa([FromBody] EnableMfaRequest request)
    {
        try
        {
            // Extraer userId del request
            int userId = request.UserId;
            
            // Buscar el usuario para generar MFA usando la query correcta
            var getUserQuery = new GetUserByIdQuery(userId);
            var user = await userQueryService.Handle(getUserQuery);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });
            
            // Generar clave secreta para TOTP
            var key = KeyGeneration.GenerateRandomKey(20);
            var secretKey = Base32Encoding.ToString(key);
            
            // Actualizar usuario con MfaKey
            user.UpdateMfaKey(secretKey);
            await unitOfWork.CompleteAsync(); // Persistir cambios
            
            // Crear URL otpauth para el código QR
            string otpauthUrl = $"otpauth://totp/AlquilaFacil:{user.Username}?secret={secretKey}&issuer=AlquilaFacil";
            
            return Ok(new
            {
                success = true,
                secretKey,
                otpauthUrl,
                message = "MFA habilitado correctamente. Escanea el código QR con Google Authenticator."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}