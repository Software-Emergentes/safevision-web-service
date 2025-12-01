using SafeVisionPlatform.IAM.Domain.Model.Aggregates;

namespace SafeVisionPlatform.IAM.Domain.Model.ValueObjects;

/// <summary>
/// Objeto de valor que representa el resultado de una autenticación
/// </summary>
public class AuthenticationResult
{
    private AuthenticationResult() { }
    
    public User? User { get; private set; }
    public string? Token { get; private set; }
    public bool RequiresMfa { get; private set; }
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Crea un resultado exitoso con autenticación completa
    /// </summary>
    public static AuthenticationResult CreateSuccessful(User user, string token)
    {
        return new AuthenticationResult
        {
            User = user,
            Token = token,
            RequiresMfa = false,
            Success = true
        };
    }

    /// <summary>
    /// Crea un resultado que requiere verificación MFA adicional
    /// </summary>
    public static AuthenticationResult CreateRequiresMfa(User user)
    {
        return new AuthenticationResult
        {
            User = user,
            RequiresMfa = true,
            Success = true
        };
    }

    /// <summary>
    /// Crea un resultado fallido con un mensaje de error
    /// </summary>
    public static AuthenticationResult CreateFailed(string errorMessage)
    {
        return new AuthenticationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
