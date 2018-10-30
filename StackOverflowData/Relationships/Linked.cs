using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.StackOverflowEntities;

namespace StackOverflowData.Relationships {
    public class Linked {
        public int QuestionId { get; private set; }
        public Question Question { get; private set; }

        public int LinkpostId { get; private set; }
        public Question Linkpost { get; private set; }
    }

    internal class LinkedConfiguration : IEntityTypeConfiguration<Linked> {
        public void Configure(EntityTypeBuilder<Linked> builder) {
            builder.ToTable("linked");
            builder.Property(x => x.QuestionId).HasColumnName("question_id");
            builder.Property(x => x.LinkpostId).HasColumnName("linkpost_id");
            builder.HasOne(l => l.Question)
                .WithMany(q => q.Linkposts)
                .HasForeignKey(l => l.QuestionId);
            builder.HasOne(l => l.Linkpost)
                .WithMany(lp => lp.Linkposts)
                .HasForeignKey(l => l.LinkpostId);
        }
    }
}