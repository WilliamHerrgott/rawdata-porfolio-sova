using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Relationships {
    public class QuestionsAnswers {
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

    internal class QuestionsAnswersConfiguration : IEntityTypeConfiguration<QuestionsAnswers> {
        public void Configure(EntityTypeBuilder<QuestionsAnswers> builder) {
            builder.ToTable("questions_answers");
            builder.Property(x => x.AnswerId).HasColumnName("answer_id");
            builder.Property(x => x.QuestionId).HasColumnName("question_id");
            builder.HasOne(qa => qa.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(qa => qa.QuestionId);
            builder.HasOne(qa => qa.Answer)
                .WithOne(a => a.Question)
                .HasForeignKey<QuestionsAnswers>(a => a.AnswerId);
        }
    }
}