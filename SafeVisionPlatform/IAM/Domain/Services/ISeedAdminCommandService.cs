using SafeVisionPlatform.IAM.Domain.Model.Commands;

namespace SafeVisionPlatform.IAM.Domain.Services;

public interface ISeedAdminCommandService
{
    Task Handle(SeedAdminCommand command);
}