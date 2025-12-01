namespace SafeVisionPlatform.IAM.Domain.Model.Commands;

public record VerifyMfaCommand(int UserId, string Code, DateTime CreatedAt);
