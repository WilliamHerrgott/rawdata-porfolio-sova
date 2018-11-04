using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.SOVAEntities;
using StackOverflowData.StackOverflowEntities;

namespace StackOverflowData.Relationships {
    public class Marks {
        public int UserId { get; set; }
        public SOVAUser User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public DateTime MarkCreationDate { get; set; }
        public DateTime AnnotationCreationDate { get; set; }
        public string AnnotationText { get; set; }
    }

    internal class MarksConfiguration : IEntityTypeConfiguration<Marks> {
        public void Configure(EntityTypeBuilder<Marks> builder) {
            builder.ToTable("marks");
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.Property(x => x.MarkCreationDate).HasColumnName("marked_creationdate");
            builder.Property(x => x.AnnotationCreationDate).HasColumnName("annotation_creationdate");
            builder.HasKey(m => new {m.UserId, m.PostId});
            builder.HasOne(m => m.User)
                .WithMany(u => u.Marks)
                .HasForeignKey(m => m.UserId);
            builder.HasOne(m => m.Post)
                .WithMany(p => p.ByUser)
                .HasForeignKey(m => m.PostId);
        }
    }
}