using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData
{
    class Comment
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Body { get; set; }
        public DateTime CreationDate { get; set; }
    }

    class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("comments");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Score).HasColumnName("score");
            builder.Property(x => x.Body).HasColumnName("body");
            builder.Property(x => x.CreationDate).HasColumnName("creation_date");
            //DateTime.ParseExact(, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
