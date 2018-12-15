using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StackOverflowData.Functions;
using StackOverflowData.Relationships;
using StackOverflowData.SOVAEntities;
using StackOverflowData.StackOverflowEntities;

namespace StackOverflowData {
    public class StackOverflowContext : DbContext {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Question> Questions { get; set; }

        public DbSet<Linked> Links { get; set; }

        public DbQuery<GetPostOrCommentResult> GetPostResults { get; set; }
        public DbQuery<SearchResult> SearchResults { get; set; }
        public DbQuery<SearchResult> SearchResultsWords { get; set; }
        public DbQuery<GetAuthorResult> GetAuthorResult { get; set; }

        //SOVA DbSets

        public DbSet<SOVAUser> Users { get; set; }
        public DbSet<History> History { get; set; }

        public DbSet<Marks> Marks { get; set; }

        public DbQuery<GetUserResult> GetUserResult { get; set; }
        public DbQuery<BooleanResult> BooleanResult { get; set; }
        public DbQuery<GetHistoryResult> GetHistoryResult { get; set; }
        public DbQuery<GetMarkedResult> GetMarkedResult { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
//            optionsBuilder.UseNpgsql("host=rawdata.ruc.dk;db=raw9;uid=raw9;pwd=oXa+yMeV");
            optionsBuilder.UseNpgsql("host=localhost;db=sova;uid=user;pwd=postgresqlpwd");
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

            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());

            modelBuilder.ApplyConfiguration(new LinkedConfiguration());

            modelBuilder.ApplyConfiguration(new GetPostOrCommentResultConfiguration());
            modelBuilder.ApplyConfiguration(new SearchResultConfiguration());
            modelBuilder.ApplyConfiguration(new SearchResultWordsConfiguration());
            modelBuilder.ApplyConfiguration(new GetAuthorResultConfiguration());

            //SOVA configurations
            modelBuilder.ApplyConfiguration(new SOVAUserConfiguration());
            modelBuilder.ApplyConfiguration(new HistoryConfiguration());

            modelBuilder.ApplyConfiguration(new MarksConfiguration());

            modelBuilder.ApplyConfiguration(new GetUserResultConfiguration());
            modelBuilder.ApplyConfiguration(new BooleanResultConfiguration());
            modelBuilder.ApplyConfiguration(new GetHistoryResultConfiguration());
            modelBuilder.ApplyConfiguration(new GetMarkedResultConfiguration());
        }
    }
}