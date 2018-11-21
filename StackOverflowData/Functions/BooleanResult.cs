using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class BooleanResult {
        public bool Successful { get; set; }
    }

    class BooleanResultConfiguration : IQueryTypeConfiguration<BooleanResult> {
        public void Configure(QueryTypeBuilder<BooleanResult> builder) {
            builder.Property(x => x.Successful).HasColumnName("successful");
        }
    }
}