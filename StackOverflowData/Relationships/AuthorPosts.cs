using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Relationships
{
    public class AuthorPosts
    {
        public int PostId { get; set; }
        public Post Post { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    class AuthorPostsConfiguration : IEntityTypeConfiguration<AuthorPosts>
    {
        public void Configure(EntityTypeBuilder<AuthorPosts> builder)
        {
            builder.ToTable("author_posts");
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.Property(x => x.AuthorId).HasColumnName("question_id");
        }
    }
}
