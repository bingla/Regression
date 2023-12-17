using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Entities;

namespace Regression.Data.Repositories
{
    public class TestRunRepository : GeneralRepository<TestRun>, ITestRunRepository
    {
        public TestRunRepository(RegressionContext context) : base(context)
        { }
    }
}
