using SafeVisionPlatform.IAM.Domain.Model.Queries;
using SafeVisionPlatform.IAM.Domain.Services;

namespace SafeVisionPlatform.IAM.Interfaces.ACL.Service;

public class IamContextFacade(IUserQueryService userQueryService) : IIamContextFacade
{
    private readonly IUserQueryService _userQueryService = userQueryService;

    public async Task<int> FetchUserIdByUsername(string username)
    {
        var getUserByUsernameQuery = new GetUserByEmailQuery(username);
        var result = await _userQueryService.Handle(getUserByUsernameQuery);
        return result?.Id ?? 0;
    }

    public async Task<string> FetchUsernameByUserId(int userId)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await _userQueryService.Handle(getUserByIdQuery);
        return result?.Username ?? string.Empty;
    }

    public bool UsersExists(int userId)
    {
        return _userQueryService.Handle(new UserExistsQuery(userId));
    }
}