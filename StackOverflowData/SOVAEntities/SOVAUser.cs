using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowData.Relationships;

namespace StackOverflowData.SOVAEntities {
    public class SOVAUser {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Location { get; set; }
        public string Salt { get; set; }
        public List<History> Searched { get; set; }
        public List<Marks> Marks { get; set; }
    }

    internal class SOVAUserConfiguration : IEntityTypeConfiguration<SOVAUser> {
        public void Configure(EntityTypeBuilder<SOVAUser> builder) {
            builder.ToTable("SOVA_users");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Email).HasColumnName("email");
            builder.Property(x => x.Username).HasColumnName("username");
            builder.Property(x => x.Password).HasColumnName("password");
            builder.Property(x => x.Location).HasColumnName("location");
            builder.HasKey(u => u.Id);
        }
    }
}