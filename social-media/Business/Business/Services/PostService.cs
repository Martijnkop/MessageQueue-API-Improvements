using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services;
using Social.Business.Services.Base;
using Social.Domain.Abstractions;
using Social.Domain.Abstractions.Repositories;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;

namespace Social.Business.Services;

public class PostService : Service<Post, CreatePostDTO, EditPostDTO>, IPostService
{
    public PostService(IPostRepository repository, IUnitOfWork unitOfWork, ILogger<PostService> logger) : base(repository, unitOfWork, logger)
    {
    }

    protected override Post Map(CreatePostDTO entity)
    {
        return new Post
        {
            Id = Guid.CreateVersion7(),
            CreatedTime = DateTime.UtcNow,
            Title = entity.Title,
            Message = entity.Message,
        };
    }

    protected override Post Update(Post value, EditPostDTO entity)
    {
        value.Message = entity.Message ?? value.Message;
        return value;
    }
}
