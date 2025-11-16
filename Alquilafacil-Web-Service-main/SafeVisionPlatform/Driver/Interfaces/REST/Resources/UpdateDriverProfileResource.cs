namespace SafeVisionPlatform.Driver.Interfaces.REST.Resources;

/// <summary>
/// Recurso para actualizar el perfil de un conductor.
/// </summary>
public class UpdateDriverProfileResource
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int YearsExperience { get; set; }
}

