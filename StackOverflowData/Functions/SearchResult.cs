using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class SearchResult {
        public int Id { get; set; }
    }

    class SearchResultConfiguration : IQueryTypeConfiguration<SearchResult> {
        public void Configure(QueryTypeBuilder<SearchResult> builder) {
            builder.Property(x => x.Id).HasColumnName("id");
        }
    }
}