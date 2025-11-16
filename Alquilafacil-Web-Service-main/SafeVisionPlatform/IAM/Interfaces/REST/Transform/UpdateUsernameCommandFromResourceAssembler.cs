using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;

namespace SafeVisionPlatform.IAM.Interfaces.REST.Transform;

public static class UpdateUsernameCommandFromResourceAssembler
{
    public static UpdateUsernameCommand ToUpdateUsernameCommand(int id, UpdateUsernameResource resource)
    {
        return new UpdateUsernameCommand(id, resource.Username);
    }
}