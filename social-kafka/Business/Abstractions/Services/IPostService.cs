namespace Social.Business.Abstractions.Services;

public interface IPostService : IService<Post, CreatePostDTO, EditPostDTO>
{
    Task<List<GetFeedDTO>> GetFeedAsync();
    Task LikeAsync(Guid kratosId, Guid postId, CancellationToken ct);
}
