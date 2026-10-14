using Social.Domain.Abstractions.Repositories.Base;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;

namespace Social.Domain.Abstractions.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<List<GetFeedDTO>> GetFeedAsync();
}
