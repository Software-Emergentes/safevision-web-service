namespace SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

/// <summary>
/// Value Object que encapsula el número de licencia de conducir con validación de formato.
/// </summary>
public class LicenseNumber
{
    public string Value { get; private set; }

    private LicenseNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("License number cannot be empty", nameof(value));

        if (value.Length < 3 || value.Length > 20)
            throw new ArgumentException("License number must be between 3 and 20 characters", nameof(value));

        Value = value.Trim();
    }

    public static LicenseNumber Create(string value)
    {
        return new LicenseNumber(value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not LicenseNumber other) return false;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}

