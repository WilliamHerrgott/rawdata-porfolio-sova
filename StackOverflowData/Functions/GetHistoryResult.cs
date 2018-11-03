using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions
{
    public class GetHistoryResult
    {
        public int Id { get; set; }
        public string SearchedText { get; set; }
        public DateTime Date { get; set; }
    }

    class GetHistoryResultConfiguration : IQueryTypeConfiguration<GetHistoryResult>
    {
        public void Configure(QueryTypeBuilder<GetHistoryResult> builder)
        {
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.SearchedText).HasColumnName("search_text");
            builder.Property(x => x.Date).HasColumnName("date");
        }
    }
}
