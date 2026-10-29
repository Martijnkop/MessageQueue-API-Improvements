using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services;
using Social.Domain.Models;
using Social.Domain.Models.Base;
using Social.Domain.Models.DTOs;
using Social.WebApi.Mappers;
using Social.WebApi.v1.Controllers.Base;
using Social.WebApi.v1.Models.Request;
using Social.WebApi.v1.Models.Response;
using System.Security.Claims;

namespace Social.WebApi.v1.Controllers;

public class PostController : Controller<Post, PostResponse, CreatePostRequest, EditPostRequest, CreatePostDTO, EditPostDTO>
{
    private IPostService _productService;

    public PostController(
        IPostService productService,
        ILogger<PostController> logger
    ) : base(
        productService,
        new PostMapper(),
        logger)
    {
        _productService = productService;
    }

    [NonAction]
    public override Task<ActionResult<IEnumerable<PostResponse>>> GetAllAsync(CancellationToken ct)
    {
        return base.GetAllAsync(ct);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetFeedResponse>>> GetFeedAsync(CancellationToken ct)
    {
        List<GetFeedDTO> posts = await _productService.GetFeedAsync();
        return Ok(posts.Select(post => new GetFeedResponse
        {
            Id = post.Id,
            CreatedAt = post.CreatedAt,
            Title = post.Title,
            Message = post.Message,
            UserId = post.UserId,
            User = new UserMapper().Map(post.User!),
            LikeCount = post.LikeCount,
        }));
    }

    [HttpPost("like")]
    public async Task<ActionResult> Like([FromBody] Guid postId, CancellationToken ct)
    {
        var claimsUser = HttpContext.User;
        var sub = claimsUser.FindFirstValue("sub")
              ?? claimsUser.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var kratosId)) throw new NotImplementedException(); // This should not be possible, as we use the same validation in the middleware

        await _productService.LikeAsync(kratosId, postId, ct);
        return Ok();
    }
}
