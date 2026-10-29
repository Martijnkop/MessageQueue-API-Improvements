namespace Social.Business.Abstractions.Services;

public interface IPostService : IService<Post>
{
    Task<List<GetFeedDTO>> GetFeedAsync();
}
