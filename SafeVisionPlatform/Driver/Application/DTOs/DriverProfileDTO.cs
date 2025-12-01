namespace SafeVisionPlatform.Driver.Application.DTOs;

/// <summary>
/// DTO que contiene información detallada del perfil del conductor.
/// </summary>
public class DriverProfileDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int YearsExperience { get; set; }
    public bool AcceptsPromotions { get; set; }
    public bool ReceiveNotifications { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

