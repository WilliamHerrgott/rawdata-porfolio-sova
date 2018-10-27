using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Relationships
{
    public class AuthorComments
    {
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    class AuthorCommentsConfiguration : IEntityTypeConfiguration<AuthorComments>
    {
        public void Configure(EntityTypeBuilder<AuthorComments> builder)
        {
            builder.ToTable("author_comments");
            builder.Property(x => x.CommentId).HasColumnName("comment_id");
            builder.Property(x => x.AuthorId).HasColumnName("author_id");
        }
    }
}
