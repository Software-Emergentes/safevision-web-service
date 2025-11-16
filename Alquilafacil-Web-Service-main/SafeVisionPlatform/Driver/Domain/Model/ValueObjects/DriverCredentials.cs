namespace SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

/// <summary>
/// Value Object que encapsula las credenciales de acceso del conductor con reglas de seguridad.
/// </summary>
public class DriverCredentials
{
    public string Username { get; private set; }
    public string EncryptedPassword { get; private set; }

    // Constructor para EF Core
    public DriverCredentials()
    {
        Username = null!;
        EncryptedPassword = null!;
    }

    private DriverCredentials(string username, string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (username.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters", nameof(username));

        if (string.IsNullOrWhiteSpace(encryptedPassword))
            throw new ArgumentException("Password cannot be empty", nameof(encryptedPassword));

        Username = username.Trim().ToLower();
        EncryptedPassword = encryptedPassword;
    }

    public static DriverCredentials Create(string username, string encryptedPassword)
    {
        return new DriverCredentials(username, encryptedPassword);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not DriverCredentials other) return false;
        return Username == other.Username && EncryptedPassword == other.EncryptedPassword;
    }

    public override int GetHashCode() => HashCode.Combine(Username, EncryptedPassword);
}
