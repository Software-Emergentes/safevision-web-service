using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Domain.Model.Entities;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;
using SafeVisionPlatform.IAM.Domain.Respositories;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.IAM.Application.Internal.CommandServices;

public class SeedUserRoleCommandService(IUserRoleRepository repository, IUnitOfWork unitOfWork) : ISeedUserRoleCommandService
{
    public async Task Handle(SeedUserRolesCommand command)
    {
        foreach (EUserRoles role in Enum.GetValues(typeof(EUserRoles)))
        {
            if (await repository.ExistsUserRole(role)) continue;
            var userRole = new UserRole(role.ToString());
            await repository.AddAsync(userRole);
        }

        await unitOfWork.CompleteAsync();
    }
}