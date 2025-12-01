using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly EUserRoles _requiredRole;
    public EUserRoles RequiredRole => _requiredRole;
    
    public AuthorizeRoleAttribute(EUserRoles requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {

        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous) return;
        
        var user = (User?)context.HttpContext.Items["User"];
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        if (user.RoleId != (int)_requiredRole)
        {
            context.Result = new ForbidResult(); // 403 Forbidden
        }
    }
}