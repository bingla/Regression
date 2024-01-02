using Microsoft.EntityFrameworkCore;
using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Entities;

namespace Regression.Data.Repositories
{
    public class TestCollectionRepository : GeneralRepository<TestCollection>, ITestCollectionRepository
    {
        public TestCollectionRepository(RegressionContext context) : base(context)
        { }

        public Task<TestCollection?> GetTestCollectionWithTestsAsync(Guid testCollectionId)
        {
            return _dbSet
                .Include(p => p.Tests)
                .FirstOrDefaultAsync(p => p.Id == testCollectionId);
        }
    }
}
