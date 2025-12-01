using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using OtpNet;
using SafeVisionPlatform.IAM.Application.Internal.OutboundServices;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;
using SafeVisionPlatform.IAM.Domain.Respositories;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.IAM.Application.Internal.CommandServices;

public class MfaCommandService : IMfaCommandService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    
    public MfaCommandService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<AuthenticationResult> Handle(VerifyMfaCommand command)
    {
        // Buscar el usuario por ID
        var user = await _userRepository.FindByIdAsync(command.UserId);
        if (user == null)
            return AuthenticationResult.CreateFailed("Usuario no encontrado");
        
        // Verificar si el usuario tiene MFA configurado
        bool hasMfaKey = !string.IsNullOrEmpty(user.MfaKey);
        bool hasOneTimeCodes = user.MfaCodes != null && user.MfaCodes.Any();
        
        if (!hasMfaKey && !hasOneTimeCodes)
            return AuthenticationResult.CreateFailed("El usuario no tiene autenticación MFA configurada");
        
        bool isVerified = false;
        
        // Verificar con TOTP si está configurado
        if (hasMfaKey)
        {
            var totp = new Totp(Base32Encoding.ToBytes(user.MfaKey));
            isVerified = totp.VerifyTotp(command.Code, out _);
        }
        else
        {
            // Si no hay MfaKey, buscar código de un solo uso
            var existingMfaCode = user.MfaCodes?.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
            if (existingMfaCode != null && 
                existingMfaCode.CreatedAt.AddMinutes(5) >= DateTime.UtcNow && 
                existingMfaCode.Code == command.Code)
            {
                isVerified = true;
            }
        }
        
        if (isVerified)
        {
            // Si la verificación MFA es exitosa, generar token y retornar
            var token = _tokenService.GenerateToken(user);
            return AuthenticationResult.CreateSuccessful(user, token);
        }
        
        return AuthenticationResult.CreateFailed("Código MFA inválido o expirado");
    }
    
    public async Task<AuthenticationResult> GenerateMfaKey(int userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
            return AuthenticationResult.CreateFailed("Usuario no encontrado");
        
        // Generar una nueva clave secreta para TOTP
        var key = KeyGeneration.GenerateRandomKey(20);
        var base32Key = Base32Encoding.ToString(key);
        
        // Actualizar el usuario con la nueva clave MFA
        user.UpdateMfaKey(base32Key);
        await _unitOfWork.CompleteAsync();
        
        // Retornar resultado exitoso con la clave generada
        var result = AuthenticationResult.CreateSuccessful(user, base32Key);
        return result;
    }
}
