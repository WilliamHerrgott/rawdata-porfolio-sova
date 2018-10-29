using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData
{
    public class Searched
    {
        public int HistoryId { get; set; }
        public History History { get; set; }

        public int UserId { get; set; }
        public SOVAUser User { get; set; }
    }

    class SearchedConfiguration : IEntityTypeConfiguration<Searched>
    {
        public void Configure(EntityTypeBuilder<Searched> builder)
        {
            builder.ToTable("searched");
            builder.Property(x => x.HistoryId).HasColumnName("history_id");
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.HasOne(s => s.User)
                   .WithMany(u => u.Searched)
                   .HasForeignKey(qa => qa.UserId);
            builder.HasOne(s => s.History)
                   .WithOne(h => h.ByUser)
                   .HasForeignKey<Searched>(s => s.HistoryId);
        }
    }
}
