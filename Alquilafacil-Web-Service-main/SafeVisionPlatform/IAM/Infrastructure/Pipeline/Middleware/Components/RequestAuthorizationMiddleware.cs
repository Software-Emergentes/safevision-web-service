using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeVisionPlatform.IAM.Application.Internal.OutboundServices;
using SafeVisionPlatform.IAM.Domain.Model.Queries;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /**
     * InvokeAsync is called by the ASP.NET Core runtime.
     * It is used to authorize requests.
     * It validates a token is included in the request header and that the token is valid.
     * If the token is valid then it sets the user in HttpContext.Items["User"].
     */
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        Console.WriteLine("Entering InvokeAsync");

        if (context.Request.HttpContext.GetEndpoint() == null)
        {
            Console.WriteLine("Endpoint is Skipping authorization");
            await next(context);
            return;
        }
        
        // skip authorization if endpoint is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.Request.HttpContext.GetEndpoint()!.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute));
        Console.WriteLine($"Allow Anonymous is {allowAnonymous}");
        if (allowAnonymous)
        {
            Console.WriteLine("Skipping authorization");
            // [AllowAnonymous] attribute is set, so skip authorization
            await next(context);
            return;
        }
        Console.WriteLine("Entering authorization");
        // get token from request header
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();


        // if token is null then throw exception
        if (token == null) throw new Exception("Null or invalid token");

        // validate token
        var userId = await tokenService.ValidateToken(token);

        // if token is invalid then throw exception
        if (userId == null) throw new Exception("Invalid token");

        // get user by id
        var getUserByIdQuery = new GetUserByIdQuery(userId.Value);
        
        var user = await userQueryService.Handle(getUserByIdQuery);
        
        // verify if user has the required role
        var authorizeRole = context.Request.HttpContext.GetEndpoint()!.Metadata
            .OfType<AuthorizeRoleAttribute>()
            .FirstOrDefault();
        if(authorizeRole != null && user?.RoleId != (int)authorizeRole.RequiredRole)
        {
            throw new Exception("User does not have the required role");
        }

        // set user in HttpContext.Items["User"]
        Console.WriteLine("Successful authorization. Updating Context...");
        context.Items["User"] = user;
        Console.WriteLine("Continuing with Middleware Pipeline");
        // call next middleware
        await next(context);
    }
}