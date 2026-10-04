using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;
using Social.WebApi.Mappers;
using Social.WebApi.v1.Controllers.Base;
using Social.WebApi.v1.Models.Request;
using Social.WebApi.v1.Models.Response;

namespace Social.WebApi.v1.Controllers;

public class PostController : Controller<Post, PostResponse, CreatePostRequest, EditPostRequest, CreatePostDTO, EditPostDTO>
{
    public PostController(
        IPostService productService,
        ILogger<PostController> logger
    ) : base(
        productService,
        new PostMapper(),
        logger)
    {
    }
}
