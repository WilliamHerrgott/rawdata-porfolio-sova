using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData
{
    public class Answer
    {
        public int Id { get; private set; }
        public Question Question { get; private set; }
    }

    class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable("answers");
            builder.Property(x => x.Id).HasColumnName("id");
        }
    }
}
