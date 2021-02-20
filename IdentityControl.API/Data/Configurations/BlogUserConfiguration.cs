using IdentityControl.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityControl.API.Data.Configurations
{
    public class BlogUserConfiguration : IEntityTypeConfiguration<BlogOwner>
    {
        public void Configure(EntityTypeBuilder<BlogOwner> builder)
        {
            builder.HasKey(x => new {x.UserId, x.BlogId});
            builder.HasOne(x => x.Blog)
                .WithMany(x => x.Owners)
                .HasForeignKey(x => x.BlogId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.OwnedBlogs)
                .HasForeignKey(x => x.UserId);
        }
    }
}