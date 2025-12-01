using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;

namespace SafeVisionPlatform.IAM.Interfaces.REST.Transform;

public static class VerifyMfaFromResource
{
    public static VerifyMfaCommand ToCommandFromResource(VerifyMfaCodeRequest resource)
    {
        return new VerifyMfaCommand(resource.UserId, resource.Code, DateTime.UtcNow);
    }
}