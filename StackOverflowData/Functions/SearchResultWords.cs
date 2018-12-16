using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions
{
    public class SearchResultWords
    {
        public string Word { get; set; }
        public double Weight { get; set; }
    }

    class SearchResultWordsConfiguration : IQueryTypeConfiguration<SearchResultWords>
    {
        public void Configure(QueryTypeBuilder<SearchResultWords> builder)
        {
            builder.Property(x => x.Weight).HasColumnName("weight");
            builder.Property(x => x.Word).HasColumnName("word");
        }
    }
}
