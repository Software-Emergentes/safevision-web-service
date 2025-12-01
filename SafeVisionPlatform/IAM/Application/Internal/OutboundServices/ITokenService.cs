using SafeVisionPlatform.IAM.Domain.Model.Aggregates;

namespace SafeVisionPlatform.IAM.Application.Internal.OutboundServices;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<int?> ValidateToken(string token);
}