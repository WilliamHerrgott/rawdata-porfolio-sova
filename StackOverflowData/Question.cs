using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData {
    public class Question : Post {
        //public int Id { get; private set; }
        public int AcceptedAnswerId { get; private set; }
        public DateTime ClosedDate { get; private set; }
        public string Title { get; private set; }
        public string Tags { get; private set; }
        public List<QuestionsAnswers> Answers { get; private set; }
        public List<Linked> Linkposts { get; private set; }
    }

    internal class QuestionConfiguration : IEntityTypeConfiguration<Question> {
        public void Configure(EntityTypeBuilder<Question> builder) {
            builder.ToTable("questions");
            //builder.Property(x => x.Id).HasColumnName("id");
            builder.HasBaseType<Post>();
            builder.Property(x => x.AcceptedAnswerId).HasColumnName("accepted_answer_id");
            builder.Property(x => x.ClosedDate).HasColumnName("closed_date");
            builder.Property(x => x.Title).HasColumnName("title");
            builder.Property(x => x.Tags).HasColumnName("tags");
            //DateTime.ParseExact(, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}