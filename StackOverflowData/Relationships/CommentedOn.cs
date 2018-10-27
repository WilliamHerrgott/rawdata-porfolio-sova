using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData
{
    public class CommentedOn
    {
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }

    class CommentedOnConfiguration : IEntityTypeConfiguration<CommentedOn>
    {
        public void Configure(EntityTypeBuilder<CommentedOn> builder)
        {
            builder.ToTable("commented_on");
            builder.Property(x => x.CommentId).HasColumnName("comment_id");
            builder.Property(x => x.PostId).HasColumnName("post_id");            
        }
    }
}
