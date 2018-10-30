using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.StackOverflowEntities
{
    public class Author {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public int Age { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }

    internal class AuthorConfiguration : IEntityTypeConfiguration<Author> {
        public void Configure(EntityTypeBuilder<Author> builder) {
            builder.ToTable("SO_authors");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.CreatedDate).HasColumnName("created_date");
            builder.Property(x => x.Location).HasColumnName("location");
            builder.Property(x => x.Age).HasColumnName("age");
        }
    }
}