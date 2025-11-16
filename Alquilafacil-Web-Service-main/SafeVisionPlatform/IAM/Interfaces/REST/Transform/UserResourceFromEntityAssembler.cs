using System.Diagnostics;
using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;

namespace SafeVisionPlatform.IAM.Interfaces.REST.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User? user)
    {
        Console.WriteLine("User Name is " + user?.Username);
        Debug.Assert(user != null, nameof(user) + " != null");
        return new UserResource(user.Id, user.Username);
    }
}