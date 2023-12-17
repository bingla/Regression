using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Entities;

namespace Regression.Data.Repositories
{
    public class RunRepository : GeneralRepository<TestRun>, IRunRepository
    {
        public RunRepository(RegressionContext context) : base(context)
        { }
    }
}
