using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class GetUserResult {
        public int Id { get; set; }
    }

    class GetUserResultConfiguration : IQueryTypeConfiguration<GetUserResult> {
        public void Configure(QueryTypeBuilder<GetUserResult> builder) {
            builder.Property(x => x.Id).HasColumnName("id");
        }
    }
}