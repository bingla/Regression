using Hangfire;
using Microsoft.EntityFrameworkCore;
using Regression.Application.Interfaces;
using Regression.Data.Interfaces;
using Regression.Domain.Entities;
using Regression.Domain.Models.Results;
using Regression.Domain.Models.Settings;

namespace Regression.Application.Services
{
    public class TestRunService : ITestRunService
    {
        private readonly IRequestService _requestService;
        private readonly ICacheRepository _cacheRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITestRunRepository _testRunRepository;
        private readonly ITestCollectionRepository _testCollectionRepository;
        private readonly ITestResultRepository _testResultRepository;

        public TestRunService(IRequestService requestService,
            ICacheRepository cacheRepository,
            IScheduleRepository scheduleRepository,
            ITestRunRepository testRunRepository,
            ITestCollectionRepository testCollectionRepository,
            ITestResultRepository testResultRepository)
        {
            _requestService = requestService;
            _cacheRepository = cacheRepository;
            _scheduleRepository = scheduleRepository;
            _testRunRepository = testRunRepository;
            _testCollectionRepository = testCollectionRepository;
            _testResultRepository = testResultRepository;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task<RunResult> RunTest(Guid scheduleId, CancellationToken token)
        {
            // Get schedule 
            var schedule = await _scheduleRepository.GetAsync(scheduleId);
            if (schedule == default)
                throw new Exception();

            // Get TestCollection (and all tests) on this schedule
            var testCollection = await _testCollectionRepository.Get(e => e.Include(p => p.Tests))
                .FirstOrDefaultAsync(e => e.Id == schedule.TestCollectionId);
            if (testCollection == default)
                throw new Exception();

            // TODO: Create a new Run and connect Run to TestCollection
            var testRunId = Guid.NewGuid();
            var testRun = new TestRun
            {
                Id = testRunId,
                TestCollectionId = schedule.TestCollectionId,
                ScheduledAt = DateTime.Now,
                RunStart = DateTime.Now,
            };

            // TODO: Create a new Hub (using hub id from test run)
            var hubId = Guid.NewGuid();

            // TODO: Connect to hub

            // TODO: Mark Test Run as started in DB
            testRun = await _testRunRepository.CreateAsync(testRun);

            // TODO: Start test
            // - For each iteration...
            //  - Start sending requests to test endpoints
            //    - In Response-handler:
            //     - Save test results in cache
            //     - Send test results to hub (or from here if loaded from cache)
            var numAgents = testCollection.NumAgents; // Num agents is the number of calls each endpoints gets per iteration
            var numIterations = testCollection.NumIterations; // Number of iterations each 
            var numMillisecondsToDelay = 1000; // Number of seconds to wait between calls, in milliseconds
            var runSettings = new RunSettings(testRunId, hubId, testCollection.AppId, testCollection.AppSecret, testCollection.XApiKey, token);
            var requests = new List<Task<HttpResponseMessage>>();

            try
            {
                for (var iteration = 0; numIterations < iteration; iteration++)
                {
                    for (var agent = 0; numAgents < agent; agent++)
                    {
                        testCollection.Tests.AsParallel().ForAll((test) =>
                        {
                            // Send request
                            requests.Add(_requestService.SendAsync(test.Method, test.Uri, test.Id, runSettings));
                        });
                    }

                    // Wait here until it's time to fire the next iteration of requests
                    await Task.Delay(numMillisecondsToDelay, token);
                }

                // Wait here until all requests are done but throw if cancellation requested
                token.ThrowIfCancellationRequested();
                await Task.WhenAll(requests.ToArray());
            }
            catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
            {
                testRun.Aborted = true;
                testRun = await _testRunRepository.UpdateAsync(testRun);
                throw;
            }
            finally
            {
                // TODO: Disconnect from Hub
            }

            // TODO: Fetch TestResult from cache
            var runResult = _cacheRepository.GetTestRun(testRunId);

            // TODO: Compile TestResults
            var testResults = runResult.Select(p => new Domain.Entities.TestResult
            {
                Id = p.InstanceId,
                RunId = p.RunId,
                TestId = p.TestId,
                RequestTime = p.RequestTime,
                Run = testRun,
            })
            .ToList();

            // TODO: Save TestResults  to DB
            await _testResultRepository.CreateAsync(testResults);

            // TODO: Compile TestResults and add to Run
            runResult.AsParallel().ForAll(p =>
            {
                testRun.Results.Add(new Domain.Entities.TestResult
                {
                    Id = p.InstanceId,
                    RunId = p.RunId,
                    TestId = p.TestId,
                    RequestTime = p.RequestTime,
                    Run = testRun,
                });
            });

            // TODO: Mark Test run as finished and save to DB
            testRun.Completed = true;
            testRun.RunEnd = DateTime.Now;
            testRun = await _testRunRepository.UpdateAsync(testRun);

            // TODO: Purge test run from cache
            _cacheRepository.PurgeTestRun(testRunId);

            return await Task.FromResult(new RunResult());
        }
    }
}
