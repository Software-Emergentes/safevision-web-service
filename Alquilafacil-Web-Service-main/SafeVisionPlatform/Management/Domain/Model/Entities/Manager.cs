namespace SafeVisionPlatform.Management.Domain.Model.Entities;

/// <summary>
/// Representa al gerente o supervisor con acceso a los reportes completos
/// y a las funcionalidades administrativas del sistema.
/// </summary>
public class Manager
{
    public int Id { get; private set; }

    /// <summary>
    /// ID del usuario asociado en el sistema IAM.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Nombre completo del gerente.
    /// </summary>
    public string FullName { get; private set; } = string.Empty;

    /// <summary>
    /// Email del gerente.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Rol del gerente en el sistema.
    /// </summary>
    public ManagerRole Role { get; private set; }

    /// <summary>
    /// IDs de las flotas que supervisa.
    /// </summary>
    public List<int> ManagedFleetIds { get; private set; } = new();

    /// <summary>
    /// Indica si el gerente está activo.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Fecha de última actualización.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Preferencias de notificación.
    /// </summary>
    public NotificationPreferences NotificationPreferences { get; private set; } = new();

    // Propiedades de persistencia para NotificationPreferences
    public bool NotifyReceiveCriticalAlerts { get; private set; } = true;
    public bool NotifyReceiveDailyReports { get; private set; } = true;
    public bool NotifyReceiveWeeklyReports { get; private set; } = true;
    public bool NotifyEmailEnabled { get; private set; } = true;
    public bool NotifyPushEnabled { get; private set; } = true;
    public bool NotifySmsEnabled { get; private set; } = false;

    private Manager() { }

    public Manager(int userId, string fullName, string email, ManagerRole role)
    {
        UserId = userId;
        FullName = fullName;
        Email = email;
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string email, string? phone)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignFleet(int fleetId)
    {
        if (!ManagedFleetIds.Contains(fleetId))
        {
            ManagedFleetIds.Add(fleetId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveFleet(int fleetId)
    {
        ManagedFleetIds.Remove(fleetId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotificationPreferences(NotificationPreferences preferences)
    {
        NotificationPreferences = preferences;
        NotifyReceiveCriticalAlerts = preferences.ReceiveCriticalAlerts;
        NotifyReceiveDailyReports = preferences.ReceiveDailyReports;
        NotifyReceiveWeeklyReports = preferences.ReceiveWeeklyReports;
        NotifyEmailEnabled = preferences.EmailNotifications;
        NotifyPushEnabled = preferences.PushNotifications;
        NotifySmsEnabled = preferences.SmsNotifications;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Roles de gerente en el sistema.
/// </summary>
public enum ManagerRole
{
    FleetManager = 1,       // Gerente de flota
    Supervisor = 2,         // Supervisor
    Administrator = 3,      // Administrador
    SafetyOfficer = 4       // Oficial de seguridad
}

/// <summary>
/// Preferencias de notificación del gerente.
/// </summary>
public class NotificationPreferences
{
    public bool ReceiveCriticalAlerts { get; set; } = true;
    public bool ReceiveDailyReports { get; set; } = true;
    public bool ReceiveWeeklyReports { get; set; } = true;
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
}
