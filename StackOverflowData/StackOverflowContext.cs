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
        public DbSet<Answer> Answers { get; private set; }
        public DbSet<Author> Authors { get; private set; }
        public DbSet<Comment> Comments { get; private set; }
        public DbSet<Post> Posts { get; private set; }
        public DbSet<Question> Questions { get; private set; }
        public DbSet<AuthorComments> AuthorComments { get; private set; }
        public DbSet<AuthorPosts> AuthorPosts { get; private set; }
        public DbSet<CommentedOn> PostComments { get; private set; }
        public DbSet<Linked> Links { get; private set; }
        public DbSet<QuestionsAnswers> QuestionsAnswers { get; private set; }
        public DbQuery<GetPostResult> GetPostResults { get; private set; }

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
