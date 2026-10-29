using Social.Domain.Models;
using Social.WebApi.Abstractions;
using Social.WebApi.v1.Models.Response;

namespace Social.WebApi.Mappers;

public class PostMapper : IMapper<Post, PostResponse>
{
    public PostResponse Map(Post input) => _Map(input, new List<Types>())!;

    internal static PostResponse? _Map(Post? input, List<Types> types)
    {
        if (input is null) return null;
        List<Types> excludeTypes = [.. types];

        excludeTypes.Add(Types.Post);

        return new PostResponse
        {
            Id = input.Id,
            Title = input.Title,
            Message = input.Message,
        };
    }
}
