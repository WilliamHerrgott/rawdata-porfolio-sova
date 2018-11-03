using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StackOverflowData.Functions;
using StackOverflowData.Relationships;
using StackOverflowData.StackOverflowEntities;

namespace StackOverflowData {
    class StackOverflowContext : DbContext {
        public DbSet<Answer> Answers { get; private set; }
        public DbSet<Author> Authors { get; private set; }
        public DbSet<Comment> Comments { get; private set; }
        public DbSet<Post> Posts { get; private set; }
        public DbSet<Question> Questions { get; private set; }

        public DbSet<Linked> Links { get; private set; }

        public DbQuery<GetPostOrCommentResult> GetPostResults { get; private set; }
        public DbQuery<SearchResult> SearchResults { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
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

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasSequence<int>("OrderNumbers")
            //    .StartsAt(99999)
            //    .IncrementsBy(1);

            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());

            modelBuilder.ApplyConfiguration(new LinkedConfiguration());

            modelBuilder.ApplyConfiguration(new GetPostOrCommentResultConfiguration());
            modelBuilder.ApplyConfiguration(new SearchResultConfiguration());
        }
    }
}