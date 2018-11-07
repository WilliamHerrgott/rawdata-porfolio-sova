using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions {
    public class GetUserResult {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Location { get; set; }
        public string Salt { get; set; }
    }

    class GetUserResultConfiguration : IQueryTypeConfiguration<GetUserResult> {
        public void Configure(QueryTypeBuilder<GetUserResult> builder) {
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Email).HasColumnName("email");
            builder.Property(x => x.Username).HasColumnName("username");
            builder.Property(x => x.Password).HasColumnName("password");
            builder.Property(x => x.Location).HasColumnName("location");
            builder.Property(x => x.Salt).HasColumnName("salt");
        }
    }
}