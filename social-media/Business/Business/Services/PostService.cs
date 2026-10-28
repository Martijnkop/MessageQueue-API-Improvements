namespace Social.Business.Services;

public class PostService : Service<Post, CreatePostDTO, EditPostDTO>, IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    public PostService(
        IPostRepository repository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<PostService> logger
        ) : base(repository, userRepository, unitOfWork, logger)
    {
        _postRepository = repository;
        _userRepository = userRepository;
    }

    public Task<List<GetFeedDTO>> GetFeedAsync()
    {
        return _postRepository.GetFeedAsync();
    }

    public async Task LikeAsync(Guid kratosId, Guid postId, CancellationToken ct)
    {
        Guid? userId = await _userRepository.GetIdByKeycloakIdAsync(kratosId, ct);
        if (userId == null) return;

        Like like = new Like
        {
            Id = Guid.CreateVersion7(),
            PostId = postId,
            UserId = (Guid) userId,
            CreatedTime = DateTime.UtcNow
        };

        await _postRepository.LikeAsync(like, ct);
        await _unitOfWork.SaveChangesAsync(ct);
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
