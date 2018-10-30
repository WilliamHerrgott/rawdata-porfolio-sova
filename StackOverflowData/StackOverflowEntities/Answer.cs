using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.StackOverflowEntities {
    public class Answer {
        
        public int Id { get; private set; }
        public int ParentId { get; private set; }

        public Post Post { get; set; }
        public Question Question { get; private set; }
    }

    class AnswerConfiguration : IEntityTypeConfiguration<Answer> {
        public void Configure(EntityTypeBuilder<Answer> builder) {
            builder.ToTable("answers");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ParentId).HasColumnName("parent_id");
            builder.HasOne(x => x.Post)
                .WithOne(x => x.Answer)
                .HasForeignKey<Answer>(x => x.Id);
            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.ParentId);
        }
    }
}