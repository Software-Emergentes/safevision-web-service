namespace SafeVisionPlatform.Driver.Application.DTOs;

/// <summary>
/// DTO que contiene los datos básicos del conductor.
/// </summary>
public class DriverDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int YearsExperience { get; set; }
    public DateTime RegisteredDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
