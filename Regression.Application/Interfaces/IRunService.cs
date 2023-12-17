using Regression.Domain.Models.Results;

namespace Regression.Application.Interfaces
{
    public interface IRunService
    {
        Task<RunResult> RunTest(Guid scheduleId);
        Task<RunResult> RunTest(Guid scheduleId, CancellationToken token);
    }
}
