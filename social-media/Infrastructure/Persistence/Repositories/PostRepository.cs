using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Social.Domain.Abstractions.Repositories;
using Social.Domain.Models;
using Social.Persistence.Repositories.Base;

namespace Social.Persistence.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(AppDataStore dbContext, ILogger<PostRepository> logger) : base(dbContext.Posts, logger)
    {
    }
}
