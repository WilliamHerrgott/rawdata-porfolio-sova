using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StackOverflowData.Functions;
using StackOverflowData.Relationships;
using StackOverflowData.SOVAEntities;

namespace StackOverflowData {
    class SOVAContext : DbContext {
        public DbSet<SOVAUser> Users { get; set; }
        public DbSet<History> History { get; set; }

        public DbSet<Marks> Marks { get; set; }

        public DbQuery<GetUserResult> GetUserResult { get; set; }
        public DbQuery<BooleanResult> BooleanResult { get; set; }

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
            modelBuilder.ApplyConfiguration(new SOVAUserConfiguration());
            modelBuilder.ApplyConfiguration(new HistoryConfiguration());

            modelBuilder.ApplyConfiguration(new MarksConfiguration());

            modelBuilder.ApplyConfiguration(new GetUserResultConfiguration());
            modelBuilder.ApplyConfiguration(new BooleanResultConfiguration());
        }
    }
}