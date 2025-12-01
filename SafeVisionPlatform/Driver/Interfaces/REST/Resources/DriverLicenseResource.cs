namespace SafeVisionPlatform.Driver.Interfaces.REST.Resources;

/// <summary>
/// Recurso de respuesta con información de la licencia de un conductor.
/// </summary>
public class DriverLicenseResource
{
    public int Id { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsValidated { get; set; }
    public DateTime ValidatedDate { get; set; }
    public bool IsExpired { get; set; }
}

