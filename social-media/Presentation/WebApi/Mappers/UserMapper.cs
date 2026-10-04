using Social.Domain.Models;
using Social.Domain.Models.DTOs;
using Social.WebApi.Abstractions;
using Social.WebApi.v1.Models.Request;
using Social.WebApi.v1.Models.Response;

namespace Social.WebApi.Mappers;

internal class UserMapper : IMapper<User, UserResponse, CreateUserRequest, CreateUserDTO, EditUserRequest, EditUserDTO>
{
    public CreateUserDTO Map(CreateUserRequest input) => new CreateUserDTO { Username = input.Username };
    public EditUserDTO Map(EditUserRequest input) => new EditUserDTO { };

    public UserResponse Map(User input) => _Map(input, new List<Types>())!;

    internal static UserResponse? _Map(User? input, List<Types> types)
    {
        if (input is null) return null;
        List<Types> excludeTypes = [.. types];

        excludeTypes.Add(Types.User);

        return new UserResponse
        {
            Id = input.Id,
            UserName = input.Username,
            Likes = input.Likes?.Select(i => LikeMapper._Map(i, excludeTypes)!).ToList() ?? null,
        };
    }
}
