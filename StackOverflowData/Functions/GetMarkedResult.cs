using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class GetMarkedResult {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Annotation { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime AnnotationDate { get; set; }
    }

    class GetMarkedResultConfiguration : IQueryTypeConfiguration<GetMarkedResult> {
        public void Configure(QueryTypeBuilder<GetMarkedResult> builder) {
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.Property(x => x.Annotation).HasColumnName("text_annotation");
            builder.Property(x => x.CreationDate).HasColumnName("marked_creationdate");
            builder.Property(x => x.AnnotationDate).HasColumnName("annotation_creationdate");
        }
    }
}