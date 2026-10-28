using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Domain.Models;

namespace Social.Persistence.ModelConfig;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasIndex(l => new { l.UserId, l.PostId }).IsUnique();
    }
}
