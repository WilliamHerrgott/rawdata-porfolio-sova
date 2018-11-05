using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class IntegerResult {
        public int Id { get; set; }
    }

    class IntegerResultConfiguration : IQueryTypeConfiguration<IntegerResult> {
        public void Configure(QueryTypeBuilder<IntegerResult> builder) {
            builder.Property(x => x.Id).HasColumnName("create_user");
        }
    }
}