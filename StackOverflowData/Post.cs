using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }      
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
