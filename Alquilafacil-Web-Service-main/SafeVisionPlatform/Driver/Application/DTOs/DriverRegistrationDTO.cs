namespace SafeVisionPlatform.Driver.Application.DTOs;

/// <summary>
/// DTO que transporta todos los datos necesarios en el proceso de registro inicial de un conductor.
/// </summary>
public class DriverRegistrationDTO
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

