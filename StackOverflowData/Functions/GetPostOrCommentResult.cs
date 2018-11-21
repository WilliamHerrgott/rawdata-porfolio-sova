using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class GetPostOrCommentResult {
        public int Id { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public DateTime CreationDate { get; set; }
        public int AuthorId { get; set; }
    }

    class GetPostOrCommentResultConfiguration : IQueryTypeConfiguration<GetPostOrCommentResult> {
        public void Configure(QueryTypeBuilder<GetPostOrCommentResult> builder) {
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Body).HasColumnName("body");
            builder.Property(x => x.Score).HasColumnName("score");
            builder.Property(x => x.CreationDate).HasColumnName("creation_date");
            builder.Property(x => x.AuthorId).HasColumnName("author_id");
        }
    }
}