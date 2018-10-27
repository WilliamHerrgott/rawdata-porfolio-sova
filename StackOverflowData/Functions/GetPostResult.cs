using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions
{
    public class GetPostResult
    {
        public string Body { get; set; }
        public string Score { get; set; }
        public DateTime CreationDate { get; set; }
    }

    class GetPostResultConfiguration : IQueryTypeConfiguration<GetPostResult>
    {
        public void Configure(QueryTypeBuilder<GetPostResult> builder)
        {
            builder.Property(x => new { x.Body, x.Score, x.CreationDate }).HasColumnName("id");
        }
    }
}
