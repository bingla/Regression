using Regression.Data.Interfaces;
using Regression.Domain.Models.Results;
using System.Collections.Concurrent;

namespace Regression.Data.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private ConcurrentDictionary<Guid, HashSet<TestResult>> _testResultCache = new();
        private ConcurrentDictionary<Guid, HashSet<TestResultAggregate>> _testAggregate = new();

        public void AddTestResult(Guid runId, TestResult newResult)
        {
            _ = _testResultCache.AddOrUpdate(runId, new HashSet<TestResult> { newResult },
                (_, hashSet) =>
                {
                    var oldResult = hashSet.FirstOrDefault(p => p.InstanceId == newResult.InstanceId);
                    if (oldResult != default)
                    {
                        oldResult = newResult;
                    }
                    else
                    {
                        hashSet.Add(newResult);
                    }

                    return hashSet;
                });
        }

        public TestResult? GetTestResult(Guid runId, Guid instanceId)
        {
            return GetTestRun(runId).FirstOrDefault(p => p.InstanceId == instanceId);
        }

        public HashSet<TestResult> GetTestRun(Guid runId)
        {
            return _testResultCache.TryGetValue(runId, out var value)
                ? value
                : [];
        }

        HashSet<TestResult> ICacheRepository.GetAllTestRuns()
        {
            return _testResultCache.Values.SelectMany(p => p).ToHashSet();
        }

        public void PurgeTestRun(Guid runId)
        {
            if (_testResultCache.ContainsKey(runId))
            {
                _testResultCache.Remove(runId, out _);
            }
        }
    }
}
