using SafeVisionPlatform.IAM.Domain.Model.Entities;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.IAM.Domain.Respositories;

public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    Task<bool> ExistsUserRole(EUserRoles role);
}