namespace SafeVisionPlatform.Driver.Interfaces.REST.Resources;

/// <summary>
/// Recurso para actualizar la licencia de un conductor.
/// </summary>
public class UpdateDriverLicenseResource
{
    public string LicenseNumber { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

