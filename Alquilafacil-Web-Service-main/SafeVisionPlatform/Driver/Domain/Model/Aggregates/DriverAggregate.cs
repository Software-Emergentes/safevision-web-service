using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Driver.Domain.Model.Aggregates;

/// <summary>
/// Agregado raíz que agrupa las entidades y objetos de valor relacionados con un conductor.
/// Controla las operaciones de registro, actualización de perfil y gestión de licencia.
/// </summary>
public class DriverAggregate
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DriverStatus Status { get; private set; }
    public DriverCredentials Credentials { get; private set; }
    public ContactInformation ContactInformation { get; private set; }
    public DriverProfile Profile { get; private set; }
    public DriverLicense License { get; private set; }
    public DateTime RegisteredDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // EF Constructor
    private DriverAggregate()
    {
        Status = null!;
        Credentials = null!;
        ContactInformation = null!;
        Profile = null!;
        License = null!;
    }

    public DriverAggregate(
        int userId,
        DriverCredentials credentials,
        ContactInformation contactInformation,
        DriverProfile profile,
        DriverLicense license)
    {
        UserId = userId;
        Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        ContactInformation = contactInformation ?? throw new ArgumentNullException(nameof(contactInformation));
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
        License = license ?? throw new ArgumentNullException(nameof(license));
        Status = DriverStatus.Inactive();
        RegisteredDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    // Constructor adicional que coincide con las propiedades mapeadas para que EF Core pueda hacer constructor binding
    public DriverAggregate(
        int id,
        int userId,
        DriverStatus status,
        DriverCredentials credentials,
        ContactInformation contactInformation,
        DriverProfile profile,
        DriverLicense license,
        DateTime registeredDate,
        DateTime updatedDate)
    {
        Id = id;
        UserId = userId;
        Status = status ?? throw new ArgumentNullException(nameof(status));
        Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        ContactInformation = contactInformation ?? throw new ArgumentNullException(nameof(contactInformation));
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
        License = license ?? throw new ArgumentNullException(nameof(license));
        RegisteredDate = registeredDate;
        UpdatedDate = updatedDate;
    }

    public void Activate()
    {
        if (!License.IsValid())
            throw new InvalidOperationException("Cannot activate driver without a valid license");

        Status = DriverStatus.Active();
        UpdatedDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = DriverStatus.Inactive();
        UpdatedDate = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = DriverStatus.Suspended();
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetInTrip()
    {
        if (!Status.IsActive())
            throw new InvalidOperationException("Only active drivers can be set to in trip");

        Status = DriverStatus.InTrip();
        UpdatedDate = DateTime.UtcNow;
    }

    public void EndTrip()
    {
        if (!Status.IsInTrip())
            throw new InvalidOperationException("Driver is not currently in a trip");

        Status = DriverStatus.Active();
        UpdatedDate = DateTime.UtcNow;
    }

    public void ChangeStatus(DriverStatus.Status newStatusValue)
    {
        var newStatus = DriverStatus.FromValue((int)newStatusValue);
        Status = newStatus;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber, string email, string address, int yearsExperience)
    {
        Profile.UpdateProfile(firstName, lastName, phoneNumber, email, address, yearsExperience);
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateLicense(LicenseNumber licenseNumber, string category, DateTime issuedDate, DateTime expirationDate)
    {
        License = new DriverLicense(licenseNumber, category, issuedDate, expirationDate);
        Status = DriverStatus.Inactive();
        UpdatedDate = DateTime.UtcNow;
    }

    public void ValidateLicense()
    {
        License.ValidateLicense();
        UpdatedDate = DateTime.UtcNow;
    }

    public bool CanBeAssignedToTrip() => Status.IsActive();

    public bool IsAvailable() => Status.IsActive();
}
