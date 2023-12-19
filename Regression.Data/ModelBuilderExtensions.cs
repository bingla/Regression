using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Regression.Domain.Entities;
using Regression.Domain.Models.Settings;

namespace Regression.Data
{
    internal static class ModelBuilderExtensions
    {
        public static ModelBuilder BuildModels(this ModelBuilder builder)
        {
            builder
                .Build_Test()
                .Build_TestCollection()
                .Build_Schedule()
                .Build_TestRun()
                .Build_TestResult();

            return builder;
        }

        public static ModelBuilder Build_Test(this ModelBuilder builder)
        {
            builder.Entity<Test>().HasKey(p => p.Id);
            builder.Entity<Test>()
                .HasOne(p => p.TestCollection)
                .WithMany(p => p.Tests);

            return builder;
        }

        public static ModelBuilder Build_TestCollection(this ModelBuilder builder)
        {
            builder.Entity<TestCollection>().HasKey(p => p.Id);
            builder.Entity<TestCollection>()
                .HasMany(p => p.Tests)
                .WithOne(p => p.TestCollection)
                .OnDelete(DeleteBehavior.Cascade);

            return builder;
        }

        public static ModelBuilder Build_Schedule(this ModelBuilder builder)
        {
            builder.Entity<Schedule>().HasKey(p => p.Id);

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            builder.Entity<Schedule>()
                .Property(p => p.ScheduleAt)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, jsonSettings),
                    v => JsonConvert.DeserializeObject<List<ScheduleItem>>(v, jsonSettings));

            return builder;
        }

        public static ModelBuilder Build_TestRun(this ModelBuilder builder)
        {
            builder.Entity<TestRun>().HasKey(p => p.Id);
            builder.Entity<TestRun>()
                .HasOne(p => p.TestCollection)
                .WithMany(p => p.Runs);

            return builder;
        }

        public static ModelBuilder Build_TestResult(this ModelBuilder builder)
        {
            builder.Entity<TestResult>().HasKey(p => p.Id);
            builder.Entity<TestResult>()
                .HasOne(p => p.Test)
                .WithMany(p => p.Results)
                .OnDelete(DeleteBehavior.Cascade);

            return builder;
        }
    }
}
