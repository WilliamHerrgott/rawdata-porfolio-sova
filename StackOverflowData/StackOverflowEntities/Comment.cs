using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.StackOverflowEntities {
    public class Comment {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Body { get; set; }
        public DateTime CreationDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
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
            builder.HasKey(x => x.Id);
            builder.HasOne(c => c.Author)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AuthorId);
            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
        }
    }
}