namespace SafeVisionPlatform.Driver.Interfaces.REST.Resources;

/// <summary>
/// Recurso para crear un nuevo conductor.
/// </summary>
public class CreateDriverResource
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string EncryptedPassword { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int YearsExperience { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string LicenseCategory { get; set; } = null!;
    public DateTime LicenseIssuedDate { get; set; }
    public DateTime LicenseExpirationDate { get; set; }
}

