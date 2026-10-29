using Social.Domain.Models;
using Social.WebApi.Abstractions;
using Social.WebApi.v1.Models.Response;

namespace Social.WebApi.Mappers;

internal class LikeMapper
{
    internal static LikeResponse? _Map(Like? input, List<Types> types)
    {
        if (input is null) return null;
        List<Types> excludeTypes = [.. types];

        excludeTypes.Add(Types.Like);

        return new LikeResponse
        {
            Id = input.Id,
            Post = PostMapper._Map(input.Post, excludeTypes),
            User = UserMapper._Map(input.User, excludeTypes)
        };
    }
}
