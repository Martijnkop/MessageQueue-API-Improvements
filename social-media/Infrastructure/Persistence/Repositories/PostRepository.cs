using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Social.Domain.Abstractions.Repositories;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;
using Social.Persistence.Repositories.Base;

namespace Social.Persistence.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(AppDataStore dbContext, ILogger<PostRepository> logger) : base(dbContext.Posts, logger)
    {
    }

    public Task<List<GetFeedDTO>> GetFeedAsync()
    {
        return _dbSet
            .OrderByDescending(x => x.CreatedTime)
            .Take(20)
            .Select(post => new GetFeedDTO
        {
            Id = post.Id,
            CreatedAt = post.CreatedTime,
            Title = post.Title,
            Message = post.Message,
            UserId = post.UserId,
            User = post.User,
            LikeCount = post.Likes!.Count(),
        }).AsNoTracking().ToListAsync();
    }
}
