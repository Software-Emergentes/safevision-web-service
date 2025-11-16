using SafeVisionPlatform.IAM.Domain.Model.Commands;

namespace SafeVisionPlatform.IAM.Domain.Services;

public interface ISeedUserRoleCommandService
{
    Task Handle(SeedUserRolesCommand command);
}