﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData.StackOverflowEntities {
    public class Question {
        public int Id { get; set; }
        public int AcceptedAnswerId { get; set; }
        public DateTime ClosedDate { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }

        public List<Answer> Answers { get; set; }
        public List<Linked> QuestionLinkpost { get; set; }
        public List<Linked> LinkpostQuestion { get; set; }

        public Post Post { get; set; }
    }

    internal class QuestionConfiguration : IEntityTypeConfiguration<Question> {
        public void Configure(EntityTypeBuilder<Question> builder) {
            builder.ToTable("questions");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.AcceptedAnswerId).HasColumnName("accepted_answer_id");
            builder.Property(x => x.ClosedDate).HasColumnName("closed_date");
            builder.Property(x => x.Title).HasColumnName("title");
            builder.Property(x => x.Tags).HasColumnName("tags");
            builder.HasOne(x => x.Post)
                .WithOne(x => x.Question)
                .HasForeignKey<Question>(x => x.Id);
            //DateTime.ParseExact(, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}