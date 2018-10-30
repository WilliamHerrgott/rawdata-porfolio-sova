using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.StackOverflowEntities
{
    public class Comment {
        public int Id { get; private set; }
        public int Score { get; private set; }
        public string Body { get; private set; }
        public DateTime CreationDate { get; private set; }

        public int AuthorId { get; private set; }
        public Author Author { get; private set; }

        public int PostId { get; private set; }
        public Post Post { get; private set; }
    }

    internal class CommentConfiguration : IEntityTypeConfiguration<Comment> {
        public void Configure(EntityTypeBuilder<Comment> builder) {
            builder.ToTable("comments");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Score).HasColumnName("score");
            builder.Property(x => x.Body).HasColumnName("body");
            builder.Property(x => x.CreationDate).HasColumnName("creation_date");
            builder.Property(x => x.AuthorId).HasColumnName("author_id");
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.HasOne(c => c.Author)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AuthorId);
            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
            //DateTime.ParseExact(, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}