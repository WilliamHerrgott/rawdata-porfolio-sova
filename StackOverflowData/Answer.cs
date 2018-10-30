using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData {
    public class Answer : Post {
        //public int Id { get; private set; }
        public QuestionsAnswers Question { get; private set; }
    }

    internal class AnswerConfiguration : IEntityTypeConfiguration<Answer> {
        public void Configure(EntityTypeBuilder<Answer> builder) {
            //builder.ToTable("answers");
            builder.HasBaseType<Post>();
            //builder.Property(x => x.Id).HasColumnName("id");
        }
    }
}