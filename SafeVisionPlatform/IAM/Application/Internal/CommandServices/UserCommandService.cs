using SafeVisionPlatform.IAM.Infrastructure.Hashing.BCrypt.Services;
using OtpNet;
using SafeVisionPlatform.IAM.Application.Internal.OutboundServices;
using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Domain.Model.Entities;
using SafeVisionPlatform.IAM.Domain.Respositories;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByEmailAsync(command.Email);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash) ||
            !command.Email.Contains('@'))
            throw new Exception("Invalid email or password");

        var token = tokenService.GenerateToken(user);

        return (user, token);
    }

    public async Task<User?> Handle(SignUpCommand command)
    {
        const string symbols = "!@#$%^&*()_-+=[{]};:>|./?";
        if (command.Password.Length < 8 || !command.Password.Any(char.IsDigit) || !command.Password.Any(char.IsUpper) ||
            !command.Password.Any(char.IsLower) || !command.Password.Any(c => symbols.Contains(c)))
            throw new Exception(
                "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit and one special character");
        
        if(!command.Email.Contains('@'))
            throw new Exception("Invalid email address");
        
        if (command.Phone.Length < 9)
            throw new Exception("Phone number must to be valid");

        if (await userRepository.ExistsByUsername(command.Username))
            throw new Exception($"Username {command.Username} is already taken");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Username, hashedPassword, command.Email);
        try
        {
            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception e)
        {
            var detailedMessage = e.InnerException?.Message ?? e.Message;
            throw new Exception($"An error occurred while creating user: {detailedMessage}", e);
        }
        
        return user;
    }

    public async Task<User?> Handle(UpdateUsernameCommand command)
    {
        var userToUpdate = await userRepository.FindByIdAsync(command.Id);
        if (userToUpdate == null)
            throw new Exception("User not found");
        var userExists = await userRepository.ExistsByUsername(command.Username);
        if (userExists)
        {
            throw new Exception("This username already exists");
        }

        userToUpdate.UpdateUsername(command.Username);
        await unitOfWork.CompleteAsync();
        return userToUpdate;
    }

    public async Task<MfaCode?> Handle(VerifyMfaCommand command)
    {
        // Buscar el usuario por ID
        var user = await userRepository.FindByIdAsync(command.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado");
        
        // Verificar si el usuario tiene MFA configurado
        bool hasMfaKey = !string.IsNullOrEmpty(user.MfaKey);
        bool hasOneTimeCodes = user.MfaCodes != null && user.MfaCodes.Any();
        
        if (!hasMfaKey && !hasOneTimeCodes)
            throw new Exception("El usuario no tiene autenticación MFA configurada");
        
        // Verificar con TOTP si está configurado
        if (hasMfaKey)
        {
            var totp = new Totp(Base32Encoding.ToBytes(user.MfaKey));
            bool isValid = totp.VerifyTotp(command.Code, out _);
            
            if (isValid)
            {
                // Si es válido, creamos un objeto MfaCode temporal para representar la verificación exitosa
                var mfaCode = new MfaCode
                {
                    UserId = user.Id,
                    Code = command.Code,
                    CreatedAt = DateTime.UtcNow
                };
                return mfaCode;
            }
            return null;
        }
        
        // Si no hay MfaKey, buscar código de un solo uso
        var existingMfaCode = user.MfaCodes?.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
        if (existingMfaCode == null)
            return null;

        if (existingMfaCode.CreatedAt.AddMinutes(5) < DateTime.UtcNow)
            return null;
        
        if (existingMfaCode.Code != command.Code)
            return null;

        return existingMfaCode;
    }
}
