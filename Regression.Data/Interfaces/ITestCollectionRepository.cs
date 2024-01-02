using Regression.Domain.Entities;

namespace Regression.Data.Interfaces
{
    public interface ITestCollectionRepository : IGeneralRepository<TestCollection>
    {
        Task<TestCollection?> GetTestCollectionWithTestsAsync(Guid testCollectionId);
    }
}
