using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Respositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.IAM.Infrastructure.Persistence.EFC.Respositories;

/**
 * <summary>
 *     The user repository
 * </summary>
 * <remarks>
 *     This repository is used to manage users
 * </remarks>
 */
public class UserRepository (AppDbContext context) : BaseRepository<User>(context), IUserRepository 
{
    /**
     * <summary>
     *     Find a user by username
     * </summary>
     * <param name="email">The username to search</param>
     * <returns>The user</returns>
     */
    public async Task<User?> FindByEmailAsync(string email)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(user => user.Email == email);
    }
    
    /**
     * <summary>
     *     Check if a user exists by username
     * </summary>
     * <param name="username">The username to search</param>
     * <returns>True if the user exists, false otherwise</returns>
     */
    public Task<bool> ExistsByUsername(string username)
    {
        return Task.FromResult(Context.Set<User>().Any(user => user.Username.Equals(username)));
    }

    public async Task<string?> GetUsernameByIdAsync(int userId)
    {
        return await Context.Set<User>().Where(user => user.Id == userId).Select(user => user.Username).FirstOrDefaultAsync();
    }

    public bool ExistsById(int userId)
    {
        return Context.Set<User>().Any(user => user.Id == userId);
    }
}