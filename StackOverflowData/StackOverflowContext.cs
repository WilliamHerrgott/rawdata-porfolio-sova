using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StackOverflowData.Relationships;
using StackOverflowData.Functions;

namespace StackOverflowData
{
    class StackOverflowContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AuthorComments> AuthorComments { get; set; }
        public DbSet<AuthorPosts> AuthorPosts { get; set; }
        public DbSet<CommentedOn> PostComments { get; set; }
        public DbSet<Linked> Links { get; set; }
        public DbSet<QuestionsAnswers> QuestionsAnswers { get; set; }
        public DbQuery<GetPostResult> GetPostResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("");
            // you only need this if you want to see the SQL statments created
            // by EF
            optionsBuilder.UseLoggerFactory(MyLoggerFactory)
                .EnableSensitiveDataLogging();
        }

        private static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information, true)
            });

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasSequence<int>("OrderNumbers")
            //    .StartsAt(99999)
            //    .IncrementsBy(1);
            modelBuilder.Entity<QuestionsAnswers>()
                .HasOne(qa => qa.Question)
                .WithMany(q => q.Answers);
            modelBuilder.Entity<AuthorPosts>()
                .HasOne(ap => ap.Author)
                .WithMany(p => p.Posts);
            modelBuilder.Entity<AuthorComments>()
                .HasOne(ac => ac.Author)
                .WithMany(c => c.Comments);
            modelBuilder.Entity<CommentedOn>()
                .HasOne(co => co.Post)
                .WithMany(p => p.Comments);
            modelBuilder.Entity<Linked>()
                .HasOne(l => l.Question)
                .WithMany(p => p.Linkposts);
            modelBuilder.Entity<Linked>()
                .HasOne(l => l.Linkpost)
                .WithMany(p => p.Linkposts);
            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());

            modelBuilder.ApplyConfiguration(new AuthorCommentsConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorPostsConfiguration());
            modelBuilder.ApplyConfiguration(new CommentedOnConfiguration());
            modelBuilder.ApplyConfiguration(new LinkedConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionsAnswersConfiguration());

            modelBuilder.ApplyConfiguration(new GetPostResultConfiguration());
        }
    }
}
