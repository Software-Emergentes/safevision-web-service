namespace SafeVisionPlatform.Driver.Domain.Model.Entities;

/// <summary>
/// Entidad que contiene la información de perfil del conductor.
/// Incluye foto, información de contacto adicional, preferencias y configuraciones personales.
/// </summary>
public class DriverProfile
{
    public int Id { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? PhotoUrl { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public string Address { get; private set; }
    public int YearsExperience { get; private set; }
    public bool AcceptsPromotions { get; private set; }
    public bool ReceiveNotifications { get; private set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int DriverId { get; set; }

    // EF Constructor
    private DriverProfile()
    {
        FirstName = null!;
        LastName = null!;
        PhoneNumber = null!;
        Email = null!;
        Address = null!;
    }

    public DriverProfile(string firstName, string lastName, string phoneNumber, string email, string address, int yearsExperience = 0)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));

        if (yearsExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsExperience));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email = email.Trim().ToLower();
        Address = address.Trim();
        YearsExperience = yearsExperience;
        AcceptsPromotions = false;
        ReceiveNotifications = true;
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber, string email, string address, int yearsExperience)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email = email.Trim().ToLower();
        Address = address.Trim();
        YearsExperience = yearsExperience;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdatePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentException("Photo URL cannot be empty", nameof(photoUrl));

        PhotoUrl = photoUrl.Trim();
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetPromotionPreference(bool acceptsPromotions)
    {
        AcceptsPromotions = acceptsPromotions;
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetNotificationPreference(bool receiveNotifications)
    {
        ReceiveNotifications = receiveNotifications;
        UpdatedDate = DateTime.UtcNow;
    }
}

