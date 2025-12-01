namespace SafeVisionPlatform.IAM.Domain.Model.Entities;

public partial class UserRole
{
    public int Id { get; }
    public string Role { get; set; }
}

public partial class UserRole
{
    public UserRole(string role)
    {
        Role = role;
    }

    public UserRole()
    {
        Role = string.Empty;
    }
}