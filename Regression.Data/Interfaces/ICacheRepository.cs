using Regression.Domain.Models.Results;

namespace Regression.Data.Interfaces
{
    public interface ICacheRepository
    {
        void AddTestRun(Guid runId, TestResult result);
        TestResult? GetTestResult(Guid runId, Guid instanceId);
        HashSet<TestResult> GetTestRun(Guid runId);
        HashSet<TestResult> GetAllTestRuns();
        void PurgeTestRun(Guid runId);
    }
}
