using System.Net.Mime;
using SafeVisionPlatform.IAM.Application.Internal.CommandServices;
using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.IAM.Domain.Model.Queries;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SafeVisionPlatform.IAM.Interfaces.REST.Resources;
using SafeVisionPlatform.IAM.Interfaces.REST.Transform;

namespace SafeVisionPlatform.IAM.Interfaces.REST;

/**
 * <summary>
 *     The users controller
 * </summary>
 * <remarks>
 *     This class is used to handle user requests
 * </remarks>
 */

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController(
    IUserQueryService userQueryService, IUserCommandService userCommandService
    ) : ControllerBase
{
    
    
    /**
     * <summary>
     *     Get user by id endpoint. It allows to get a user by id
     * </summary>
     * <param name="userId">The user id</param>
     * <returns>The user resource</returns>
     */
    [AuthorizeRole(EUserRoles.Admin)]
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var user = await userQueryService.Handle(getUserByIdQuery);
        var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user!);
        return Ok(userResource);
    }
    
    
    /**
     * <summary>
     *     Get all users endpoint. It allows to get all users
     * </summary>
     * <returns>The user resources</returns>
     */
    [HttpGet]
    [AuthorizeRole(EUserRoles.Admin)]
    public async Task<IActionResult> GetAllUsers()
    {
        var getAllUsersQuery = new GetAllUsersQuery();
        var users = await userQueryService.Handle(getAllUsersQuery);
        var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(userResources);
    }
    
    [HttpGet("get-username/{userId:int}")]
    public async Task<IActionResult> GetUsernameById(int userId)
    {
        var getUsernameByIdQuery = new GetUsernameByIdQuery(userId);
        var username = await userQueryService.Handle(getUsernameByIdQuery);
        return Ok(username);
    }
    
    [HttpPut("{userId:int}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUsernameResource updateUsernameResource)
    {
        var updateUserCommand =
            UpdateUsernameCommandFromResourceAssembler.ToUpdateUsernameCommand(userId, updateUsernameResource);
        var user = await userCommandService.Handle(updateUserCommand);
        var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user!);
        return Ok(userResource);
    }
}