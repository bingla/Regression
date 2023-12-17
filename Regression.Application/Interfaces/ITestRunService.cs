using Regression.Domain.Models.Results;

namespace Regression.Application.Interfaces
{
    public interface ITestRunService
    {
        Task<RunResult> RunTest(Guid scheduleId, CancellationToken token);
    }
}
