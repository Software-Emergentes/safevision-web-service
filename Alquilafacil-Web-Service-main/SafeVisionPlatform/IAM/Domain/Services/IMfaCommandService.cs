using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.IAM.Domain.Services;

public interface IMfaCommandService
{
    Task<AuthenticationResult> Handle(VerifyMfaCommand command);
    Task<AuthenticationResult> GenerateMfaKey(int userId);
}
