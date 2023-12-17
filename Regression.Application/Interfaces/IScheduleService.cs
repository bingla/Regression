using Regression.Domain.Models.Results;
using Regression.Domain.Models.Settings;

namespace Regression.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleResult> ScheduleTest(ScheduleSettings settings);
        Task DeScheduleTest(Guid scheduleId);
    }
}
