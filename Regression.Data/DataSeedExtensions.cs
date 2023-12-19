using Regression.Domain;
using Regression.Domain.Entities;

namespace Regression.Data
{
    public static class DataSeedExtensions
    {
        public static RegressionContext Seed(this RegressionContext context)
        {
            if (context.Database.EnsureCreated())
            {
                var testCollections = new List<TestCollection> {
                new TestCollection
                {
                    Id = Guid.NewGuid(),
                    Environment = Domain.Enums.Environment.Develop,
                    NumIterations = 1,
                    NumAgents = 1,
                    Name = "TestCollection 1",
                    Description = "TestCollection Description",
                    AppId = Guid.NewGuid().ToString(),
                    AppSecret = Guid.NewGuid().ToString(),
                    XApiKey = Guid.NewGuid().ToString(),
                }
            };

                var tests = new List<Test>
            {
                new Test
                {
                    Id = Guid.NewGuid(),
                    TestCollectionId = testCollections.FirstOrDefault().Id,
                    Method = Domain.Enums.HttpMethod.GET,
                    Uri = new Uri("https://api.oceandrivers.com/v1.0/getEasyWind/EW013/")
                },
                new Test
                {
                    Id = Guid.NewGuid(),
                    TestCollectionId = testCollections.FirstOrDefault().Id,
                    Method = Domain.Enums.HttpMethod.GET,
                    Uri = new Uri("https://api.oceandrivers.com/v1.0/getEasyWind/EW013/")
                },
            };

                context.TestCollections.AddRange(testCollections);
                context.Tests.AddRange(tests);
                context.SaveChanges();
            }

            return context;
        }
    }
}
