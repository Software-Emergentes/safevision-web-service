namespace SafeVisionPlatform.IAM.Domain.Model.Commands;

public record SignUpCommand(
    string Username,
    string Password,
    string Email,
    string Name,
    string FatherName,
    string MotherName,
    string DateOfBirth,
    string DocumentNumber,
    string Phone
    );