using Microsoft.EntityFrameworkCore;
using Regression.Data;
using Regression.Domain.Entities;

namespace Regression.Domain
{
    public class RegressionContext : DbContext
    {
        public DbSet<TestCollection> TestCollections { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }

        public RegressionContext(DbContextOptions<RegressionContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.BuildModels();
        }
    }
}
