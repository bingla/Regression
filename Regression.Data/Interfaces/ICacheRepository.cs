using Regression.Domain.Models.Results;

namespace Regression.Data.Interfaces
{
    public interface ICacheRepository
    {
        void AddTestResult(Guid runId, TestResult result);
        TestResult? GetTestResult(Guid runId, Guid instanceId);
        HashSet<TestResult> GetTestRun(Guid runId);
        HashSet<TestResult> GetAllTestRuns();
        void PurgeTestRun(Guid runId);
    }
}
