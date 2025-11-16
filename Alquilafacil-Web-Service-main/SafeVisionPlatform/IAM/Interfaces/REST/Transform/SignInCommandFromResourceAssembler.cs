using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;

namespace SafeVisionPlatform.IAM.Interfaces.REST.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(resource.Email, resource.Password);
    }
}