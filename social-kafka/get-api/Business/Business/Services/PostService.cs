namespace Social.Business.Services;

public class PostService : Service<Post>, IPostService
{
    private readonly IPostRepository _postRepository;
    public PostService(
        IPostRepository repository,
        ILogger<PostService> logger
        ) : base(repository, logger)
    {
        _postRepository = repository;
    }

    public Task<List<GetFeedDTO>> GetFeedAsync()
    {
        return _postRepository.GetFeedAsync();
    }
}
