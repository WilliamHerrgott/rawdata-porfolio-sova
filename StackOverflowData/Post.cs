using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData
{
    public class Post
    {
        public int Id { get; private set; }
        public DateTime? CreationDate { get; private set; }
        public string Body { get; private set; }
        public int Score { get; private set; }  
        public AuthorPosts Author { get; private set; }
        public List<CommentedOn> Comments { get; private set; }
        public List<Marks> ByUser { get; set; }
    }

    class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("posts");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.CreationDate).HasColumnName("creation_date");
            builder.Property(x => x.Body).HasColumnName("body");
            builder.Property(x => x.Score).HasColumnName("score");
            //DateTime.ParseExact(, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
