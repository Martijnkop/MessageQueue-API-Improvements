namespace Social.Business.Services;

public class PostService : Service<Post, CreatePostDTO, EditPostDTO>, IPostService
{
    private readonly IPostRepository _postRepository;
    public PostService(IPostRepository repository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<PostService> logger) : base(repository, userRepository, unitOfWork, logger)
    {
        _postRepository = repository;
    }

    public Task<List<GetFeedDTO>> GetFeedAsync()
    {
        return _postRepository.GetFeedAsync();
    }

    protected override Post Map(CreatePostDTO entity, Guid userId) => new Post
    {
        Id = Guid.CreateVersion7(),
        UserId = userId,
        CreatedTime = DateTime.UtcNow,
        Title = entity.Title,
        Message = entity.Message,
    };

    protected override Post Update(Post value, EditPostDTO entity) => value with {
        Message = entity.Message ?? value.Message,
    };
}
