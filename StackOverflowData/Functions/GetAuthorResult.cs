using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions
{
    public class GetAuthorResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public int? Age { get; set; }
    }

    class GetAuthorResultConfiguration : IQueryTypeConfiguration<GetAuthorResult>
    {
        public void Configure(QueryTypeBuilder<GetAuthorResult> builder)
        {
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.CreatedDate).HasColumnName("created_date");
            builder.Property(x => x.Location).HasColumnName("location");
            builder.Property(x => x.Age).HasColumnName("age");
        }
    }
}
