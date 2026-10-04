using Social.Business.Abstractions.Services.Base;
using Social.Domain.Models;
using Social.Domain.Models.DTOs;

namespace Social.Business.Abstractions.Services;

public interface IPostService : IService<Post, CreatePostDTO, EditPostDTO>
{
}
