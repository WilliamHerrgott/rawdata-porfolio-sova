using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.SOVAEntities {
    public class History {
        public int Id { get; set; }
        public string SearchText { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
            
        public SOVAUser ByUser { get; set; }
    }

    class HistoryConfiguration : IEntityTypeConfiguration<History> {
        public void Configure(EntityTypeBuilder<History> builder) {
            builder.ToTable("history");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.SearchText).HasColumnName("search_text");
            builder.Property(x => x.Date).HasColumnName("date");
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.HasOne(h => h.ByUser)
                .WithMany(u => u.Searched)
                .HasForeignKey(h => h.UserId);
        }
    }
}