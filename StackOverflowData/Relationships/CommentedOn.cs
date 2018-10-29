using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackOverflowData.Relationships {
    public class CommentedOn {
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }

    class CommentedOnConfiguration : IEntityTypeConfiguration<CommentedOn> {
        public void Configure(EntityTypeBuilder<CommentedOn> builder) {
            builder.ToTable("commented_on");
            builder.Property(x => x.CommentId).HasColumnName("comment_id");
            builder.Property(x => x.PostId).HasColumnName("post_id");
            builder.HasOne(co => co.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(co => co.PostId);
            builder.HasOne(co => co.Comment)
                .WithOne(p => p.Post)
                .HasForeignKey<CommentedOn>(co => co.CommentId);
        }
    }
}