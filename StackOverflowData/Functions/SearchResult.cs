using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class SearchResult {
        public int Id { get; set; }
        public string Body { get; set; }
    }

    class SearchResultConfiguration : IQueryTypeConfiguration<SearchResult> {
        public void Configure(QueryTypeBuilder<SearchResult> builder) {
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Body).HasColumnName("body");
        }
    }
    
    public class SearchResultWords {
        public int word { get; set; }
        public string grade { get; set; }
    }

    class SearchResultWordsConfiguration : IQueryTypeConfiguration<SearchResultWords> {
        public void Configure(QueryTypeBuilder<SearchResultWords> builder) {
            builder.Property(x => x.grade).HasColumnName("grade");
            builder.Property(x => x.word).HasColumnName("word");
        }
    }


}