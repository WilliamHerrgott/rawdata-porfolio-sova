using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData
{
    public class Comment
    {
        public int Id { get; private set; }
        public int Score { get; private set; }
        public string Body { get; private set; }
        public DateTime? CreationDate { get; private set; }
        public Author Author { get; private set; }
        public Post Post { get; private set; }
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
