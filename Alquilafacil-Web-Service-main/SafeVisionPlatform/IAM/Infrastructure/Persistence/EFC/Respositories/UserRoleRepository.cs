using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.IAM.Domain.Model.Entities;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;
using SafeVisionPlatform.IAM.Domain.Respositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.IAM.Infrastructure.Persistence.EFC.Respositories;

public class UserRoleRepository(AppDbContext context) : BaseRepository<UserRole>(context), IUserRoleRepository
{
    public async Task<bool> ExistsUserRole(EUserRoles role)
    {
        return await Context.Set<UserRole>().AnyAsync(userRole => userRole.Role == role.ToString());
    }
}
