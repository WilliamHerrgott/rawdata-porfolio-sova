using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.StackOverflowEntities;

namespace StackOverflowData.Relationships {
    public class Linked {
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int LinkpostId { get; set; }
        public Question Linkpost { get; set; }
    }

    internal class LinkedConfiguration : IEntityTypeConfiguration<Linked> {
        public void Configure(EntityTypeBuilder<Linked> builder) {
            builder.ToTable("linked");
            builder.Property(x => x.QuestionId).HasColumnName("question_id");
            builder.Property(x => x.LinkpostId).HasColumnName("linkpost_id");
            builder.HasKey(l => new { l.QuestionId, l.LinkpostId });
            builder.HasOne(l => l.Question)
                .WithMany(q => q.QuestionLinkpost)
                .HasForeignKey(l => l.QuestionId);
            builder.HasOne(l => l.Linkpost)
                .WithMany(lp => lp.LinkpostQuestion)
                .HasForeignKey(l => l.LinkpostId);
        }
    }
}