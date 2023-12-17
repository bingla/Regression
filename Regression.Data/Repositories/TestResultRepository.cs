using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Entities;

namespace Regression.Data.Repositories
{
    public class TestResultRepository : GeneralRepository<TestResult>, ITestResultRepository
    {
        public TestResultRepository(RegressionContext context) : base(context)
        { }
    }
}
