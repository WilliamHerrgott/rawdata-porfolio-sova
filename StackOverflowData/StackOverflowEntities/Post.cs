using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData.StackOverflowEntities {
    public class Post {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public int AuthorId { get; set; }

        public Author Author { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Marks> ByUser { get; set; }

        public Question Question { get; set; }
        public Answer Answer { get; set; }
    }

    internal class PostConfiguration : IEntityTypeConfiguration<Post> {
        public void Configure(EntityTypeBuilder<Post> builder) {
            builder.ToTable("posts");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.CreationDate).HasColumnName("creation_date");
            builder.Property(x => x.Body).HasColumnName("body");
            builder.Property(x => x.Score).HasColumnName("score");
            builder.Property(x => x.AuthorId).HasColumnName("author_id");
            builder.HasKey(x => x.Id);
            builder.HasOne(p => p.Author)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AuthorId);
            //DateTime.ParseExact(, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}