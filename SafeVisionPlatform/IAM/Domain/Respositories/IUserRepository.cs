using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.IAM.Domain.Respositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync (string email);
    Task<bool> ExistsByUsername(string username);
    Task<string?> GetUsernameByIdAsync(int userId);
    bool ExistsById(int userId);
}