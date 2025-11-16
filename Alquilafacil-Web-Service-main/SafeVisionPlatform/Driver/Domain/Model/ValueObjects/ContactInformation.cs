namespace SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

/// <summary>
/// Value Object que agrupa los datos de contacto del conductor.
/// Valida formato y completitud de los datos.
/// </summary>
public class ContactInformation
{
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string Address { get; private set; }

    // Constructor para EF Core
    public ContactInformation()
    {
        Phone = null!;
        Email = null!;
        Address = null!;
    }

    private ContactInformation(string phone, string email, string address)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));

        if (!email.Contains("@"))
            throw new ArgumentException("Email format is invalid", nameof(email));

        Phone = phone.Trim();
        Email = email.Trim().ToLower();
        Address = address.Trim();
    }

    public static ContactInformation Create(string phone, string email, string address)
    {
        return new ContactInformation(phone, email, address);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ContactInformation other) return false;
        return Phone == other.Phone && Email == other.Email && Address == other.Address;
    }

    public override int GetHashCode() => HashCode.Combine(Phone, Email, Address);
}
