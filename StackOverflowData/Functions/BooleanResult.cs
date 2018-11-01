using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Functions
{
    class BooleanResult
    {
        public bool Successful;
    }

    class BooleanResultConfiguration : IQueryTypeConfiguration<BooleanResult>
    {
        public void Configure(QueryTypeBuilder<BooleanResult> builder)
        {
            //builder.Property(x => x.Id).HasColumnName("id");
        }
    }
}
