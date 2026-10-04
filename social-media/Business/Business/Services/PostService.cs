namespace Social.Business.Services;

public class PostService : Service<Post, CreatePostDTO, EditPostDTO>, IPostService
{
    public PostService(IPostRepository repository, IUnitOfWork unitOfWork, ILogger<PostService> logger) : base(repository, unitOfWork, logger)
    {
    }

    protected override Post Map(CreatePostDTO entity) => new Post
    {
        Id = Guid.CreateVersion7(),
        CreatedTime = DateTime.UtcNow,
        Title = entity.Title,
        Message = entity.Message,
    };

    protected override Post Update(Post value, EditPostDTO entity) => value with {
        Message = entity.Message ?? value.Message,
    };
}
