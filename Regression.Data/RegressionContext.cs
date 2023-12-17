using Microsoft.EntityFrameworkCore;
using Regression.Data;
using Regression.Domain.Entities;

namespace Regression.Domain
{
    public class RegressionContext : DbContext
    {
        public DbSet<TestCollection> TestCollections { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestRun> Runs { get; set; }

        public RegressionContext(DbContextOptions<RegressionContext> options) : base(options)
        { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseInMemoryDatabase("RegressionDB")
        //        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.BuildModels();
        }
    }
}
