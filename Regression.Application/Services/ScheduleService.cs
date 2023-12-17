using Hangfire;
using Regression.Application.Interfaces;
using Regression.Data.Interfaces;
using Regression.Domain.Entities;
using Regression.Domain.Models.Results;
using Regression.Domain.Models.Settings;

namespace Regression.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITestCollectionRepository _testCollectionRepository;
        private readonly ITestRunService _runService;

        public ScheduleService(IScheduleRepository scheduleRepository,
            ITestCollectionRepository testCollectionRepository,
            ITestRunService runService)
        {
            _scheduleRepository = scheduleRepository;
            _testCollectionRepository = testCollectionRepository;
            _runService = runService;
        }

        public async Task<ScheduleResult> ScheduleTest(ScheduleSettings settings)
        {
            // TODO: Load TestCollection from DB
            var testCollection = await _testCollectionRepository.GetAsync(settings.TestCollectionId);
            if (testCollection == default)
                throw new Exception();

            // TODO: Create a new Schedule
            var scheduleId = Guid.NewGuid();
            var schedule = new Schedule
            {
                Id = scheduleId,
                TestCollectionId = settings.TestCollectionId,
                ScheduleAt = settings.ScheduleAt,
                Enabled = settings.Enabled,
                Recurring = settings.Recurring
            };

            // TODO: Save Schedule to DB
            schedule = await _scheduleRepository.CreateAsync(schedule);

            // TODO: Schedule in Hangfire
            if (schedule.Recurring)
            {
                // Each recuring day needs to be scheduled independently
                foreach (var item in schedule.ScheduleAt)
                {
                    var scheduleName = $"{scheduleId}_{item.Day}";
                    var cronExpression = $"{item.Time.Minute} {item.Time.Hour} * * {(int)item.Day}";
                    RecurringJob.AddOrUpdate(scheduleName, () => _runService.RunTest(scheduleId, CancellationToken.None), cronExpression);
                }
            }
            else
            {
                // Run job immediately
                BackgroundJob.Enqueue(() => _runService.RunTest(scheduleId, CancellationToken.None));
            }

            return await Task.FromResult(new ScheduleResult());
        }

        public async Task DeScheduleTest(Guid scheduleId)
        {
            var schedule = await _scheduleRepository.GetAsync(scheduleId);
            if (schedule == default)
                throw new Exception();

            if (schedule.Recurring)
            {
                foreach (var item in schedule.ScheduleAt)
                {
                    var scheduleName = $"{schedule.Id}_{item.Day}";
                    RecurringJob.RemoveIfExists(scheduleName);
                }
            }
        }
    }
}
