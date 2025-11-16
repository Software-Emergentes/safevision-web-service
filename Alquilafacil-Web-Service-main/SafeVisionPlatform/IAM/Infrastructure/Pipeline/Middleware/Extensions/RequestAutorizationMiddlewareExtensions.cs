using SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Components;

namespace SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}