using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Domain.Model.Entities;

namespace SafeVisionPlatform.IAM.Domain.Services;

public interface IUserCommandService
{
    Task<(User user, string token)> Handle(SignInCommand command);
    Task<User?> Handle(SignUpCommand command);
    Task<User?> Handle(UpdateUsernameCommand command);
    Task<MfaCode?> Handle(VerifyMfaCommand command);
}