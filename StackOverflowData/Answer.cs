using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData
{
    public class Answer
    {
        public int Id { get; set; }
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
