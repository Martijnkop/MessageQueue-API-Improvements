using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;
using Social.WebApi.Mappers;
using Social.WebApi.v1.Controllers.Base;
using Social.WebApi.v1.Models.Request;
using Social.WebApi.v1.Models.Response;
using System.Security.Claims;

namespace Social.WebApi.v1.Controllers;

public class UserController : Controller<User, UserResponse, CreateUserRequest, EditUserRequest, CreateUserDTO, EditUserDTO>
{
    private readonly IUserService _service;
    public UserController(IUserService service, ILogger<UserController> logger) : base(service, new UserMapper(), logger)
    {
        _service = service;
    }

    [NonAction]
    public override Task<ActionResult<UserResponse>> CreateAsync([FromBody] CreateUserRequest createModel, CancellationToken ct)
    {
        return base.CreateAsync(createModel, ct);
    }


    // NonAction to remove the Edit endpoint for users
    [NonAction]
    public override Task<ActionResult<UserResponse>> EditAsync(Guid id, [FromBody] EditUserRequest editUserRequest, CancellationToken ct)
    {
        return base.EditAsync(id, editUserRequest, ct);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse?>> Me(CancellationToken ct)
    {
        var claimsUser = HttpContext.User;
        var sub = claimsUser.FindFirstValue("sub")
              ?? claimsUser.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var kratosId)) throw new NotImplementedException(); // This should not be possible, as we use the same validation in the middleware

        var entity = await _service.Me(kratosId, ct);
        if (entity == null) throw new NotImplementedException();
        return Ok(_mapper.Map(entity));
    }
}
