using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;
using Social.WebApi.Mappers;
using Social.WebApi.v1.Controllers.Base;
using Social.WebApi.v1.Models.Request;
using Social.WebApi.v1.Models.Response;

namespace Social.WebApi.v1.Controllers;

public class UserController : Controller<User, UserResponse, CreateUserRequest, EditUserRequest, CreateUserDTO, EditUserDTO>
{
    public UserController(IUserService service, ILogger<UserController> logger) : base(service, new UserMapper(), logger)
    {
    }

    // NonAction to remove the Edit endpoint for users
    [NonAction]
    public override Task<ActionResult<UserResponse>> EditAsync(Guid id, [FromBody] EditUserRequest editUserRequest, CancellationToken ct)
    {
        return base.EditAsync(id, editUserRequest, ct);
    }
}
