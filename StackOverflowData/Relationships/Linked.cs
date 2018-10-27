using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Relationships
{
    public class Linked
    {
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int LinkpostId { get; set; }
        public Question Linkpost { get; set; }
    }

    class LinkedConfiguration : IEntityTypeConfiguration<Linked>
    {
        public void Configure(EntityTypeBuilder<Linked> builder)
        {
            builder.ToTable("linked");
            builder.Property(x => x.QuestionId).HasColumnName("question_id");
            builder.Property(x => x.LinkpostId).HasColumnName("linkpost_id");
        }
    }
}
