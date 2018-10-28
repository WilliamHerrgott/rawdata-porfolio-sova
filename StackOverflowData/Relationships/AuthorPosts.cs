using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Relationships
{
    public class AuthorPosts
    {
        public int PostId { get; private set; }
        public Post Post { get; private set; }

        public int AuthorId { get; private set; }
        public Author Author { get; private set; }
    }

    class AuthorPostsConfiguration : IEntityTypeConfiguration<AuthorPosts>
    {
        public void Configure(EntityTypeBuilder<AuthorPosts> builder)
        {
            builder.ToTable("author_posts");
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.Property(x => x.AuthorId).HasColumnName("question_id");
            builder.HasOne(ap => ap.Author)
                   .WithMany(a => a.Posts)
                   .HasForeignKey(ap => ap.AuthorId);
            builder.HasOne(ap => ap.Post)
                   .WithOne(p => p.Author)
                   .HasForeignKey<AuthorPosts>(ap => ap.PostId);
        }
    }
}
