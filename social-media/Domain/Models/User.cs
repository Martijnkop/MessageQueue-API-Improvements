using Social.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social.Domain.Models
{
    public record User : Entity
    {
        public required string Username { get; set; }
        public List<Post>? Posts { get; set; } = [];
        public List<Like>? Likes { get; set; } = [];
    }
}
