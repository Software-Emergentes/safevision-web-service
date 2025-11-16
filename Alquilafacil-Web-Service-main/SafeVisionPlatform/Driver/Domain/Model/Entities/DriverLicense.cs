using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Driver.Domain.Model.Entities;

/// <summary>
/// Entidad que almacena los datos de la licencia de conducir del conductor.
/// Valida la vigencia de la licencia y proporciona información de validación.
/// </summary>
public class DriverLicense
{
    public int Id { get; set; }
    public LicenseNumber LicenseNumber { get; private set; }
    public string Category { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public bool IsValidated { get; private set; }
    public DateTime ValidatedDate { get; private set; }
    public int DriverId { get; set; }

    // EF Constructor
    private DriverLicense()
    {
        LicenseNumber = null!;
        Category = null!;
    }

    public DriverLicense(LicenseNumber licenseNumber, string category, DateTime issuedDate, DateTime expirationDate)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        if (issuedDate >= expirationDate)
            throw new ArgumentException("Expiration date must be greater than issued date");

        LicenseNumber = licenseNumber;
        Category = category;
        IssuedDate = issuedDate;
        ExpirationDate = expirationDate;
        IsValidated = false;
        ValidatedDate = DateTime.MinValue;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpirationDate;

    public bool IsValid() => IsValidated && !IsExpired();

    public void ValidateLicense()
    {
        if (IsExpired())
            throw new InvalidOperationException("Cannot validate an expired license");

        IsValidated = true;
        ValidatedDate = DateTime.UtcNow;
    }

    public void UpdateExpirationDate(DateTime newExpirationDate)
    {
        if (newExpirationDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future");

        ExpirationDate = newExpirationDate;
        IsValidated = false;
        ValidatedDate = DateTime.MinValue;
    }
}

