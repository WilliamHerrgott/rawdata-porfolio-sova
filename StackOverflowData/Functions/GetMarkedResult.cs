using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions
{
    public class GetMarkedResult
    {
        public int PostId { get; set; }
        public string Annotation { get; set; }
        public DateTime CreationDate { get; set; }
    }

    class GetMarkedResultConfiguration : IQueryTypeConfiguration<GetMarkedResult>
    {
        public void Configure(QueryTypeBuilder<GetMarkedResult> builder)
        {
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.Property(x => x.Annotation).HasColumnName("text_annotation");
            builder.Property(x => x.CreationDate).HasColumnName("marked_creationdate");
        }
    }
}
