using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData
{
    public class QuestionsAnswers
    {
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

    class QuestionsAnswersConfiguration : IEntityTypeConfiguration<QuestionsAnswers>
    {
        public void Configure(EntityTypeBuilder<QuestionsAnswers> builder)
        {
            builder.ToTable("questions_answers");
            builder.Property(x => x.AnswerId).HasColumnName("answer_id");
            builder.Property(x => x.QuestionId).HasColumnName("question_id");
        }
    }
}
