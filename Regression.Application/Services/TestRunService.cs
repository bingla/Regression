using Hangfire;
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

        private readonly int _defaultAgentCount = 1;
        private readonly int _defaultIterationCount = 1;
        private readonly List<Task<HttpResponseMessage>> _requests = new List<Task<HttpResponseMessage>>();

        public TestRunService(IRequestService requestService,
            ICacheRepository cacheRepository,
            IScheduleRepository scheduleRepository,
            ITestRunRepository testRunRepository,
            ITestCollectionRepository testCollectionRepository)
        {
            _requestService = requestService;
            _cacheRepository = cacheRepository;
            _scheduleRepository = scheduleRepository;
            _testRunRepository = testRunRepository;
            _testCollectionRepository = testCollectionRepository;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task<RunResult> RunTest(Guid scheduleId, CancellationToken token)
        {
            // Get schedule 
            var schedule = await _scheduleRepository.GetAsync(scheduleId);
            if (schedule == default)
                throw new Exception();

            // Get TestCollection (and all tests) on this schedule
            var testCollection = await _testCollectionRepository.GetTestCollectionWithTestsAsync(schedule.TestCollectionId);
            if (testCollection == default)
                throw new Exception();

            // TODO: Create a new TestRun and connect to TestCollection
            var testRun = new TestRun
            {
                Id = Guid.NewGuid(),
                TestCollectionId = schedule.TestCollectionId,
                ScheduledAt = DateTime.Now,
                RunStart = DateTime.Now,
            };

            // TODO: Create a new Hub (using hub id from test run)
            var hubId = Guid.NewGuid();

            // TODO: Connect to hub

            // TODO: Start test
            // - For each iteration...
            //  - Start sending requests to test endpoints
            //    - In Response-handler:
            //     - Save test results in cache
            //     - Send test results to hub (or from here if loaded from cache)
            var numAgents = testCollection.NumAgents <= 0 ? _defaultAgentCount : testCollection.NumAgents; // Num agents is the number of calls each endpoints gets per iteration
            var numIterations = testCollection.NumIterations <= 0 ? _defaultIterationCount : testCollection.NumIterations; // Number of iterations each 
            var numMillisecondsToDelay = 1000; // Number of seconds to wait between calls, in milliseconds
            var runSettings = new RunSettings(testRun.Id, hubId, testCollection.AppId, testCollection.AppSecret, testCollection.XApiKey, token);

            try
            {
                for (var iteration = 0; iteration < numIterations; iteration++)
                {
                    for (var agent = 0; agent < numAgents; agent++)
                    {
                        testCollection.Tests.AsParallel().ForAll((test) =>
                        {
                            // Send request
                            _requests.Add(_requestService.SendAsync(test.Method, test.Uri, test.Id, runSettings));
                        });
                    }

                    // Wait here until it's time to fire the next iteration of requests
                    await Task.Delay(numMillisecondsToDelay, token);
                }

                // Wait here until all requests are done but throw if cancellation requested
                token.ThrowIfCancellationRequested();
                await Task.WhenAll(_requests.ToArray());
            }
            catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
            {
                // Disconnect from Hub

                // Try to save results even if the run was interrupted 
                testRun = await CompileAndSaveTestRun(testRun, false);
                throw;
            }

            // Disconnect from Hub

            // Compile and Save the result
            testRun = await CompileAndSaveTestRun(testRun, true);

            return await Task.FromResult(new RunResult());
        }

        internal async Task<TestRun> CompileAndSaveTestRun(TestRun testRun, bool testRunCompleted)
        {
            // Fetch TestResult from cache
            var runResult = _cacheRepository.GetTestRun(testRun.Id);

            // Compile TestResults and add to TestRun
            if (runResult != default && runResult.Count != 0)
            {
                // Group by test
                var groupByTest = runResult
                    .GroupBy(p => p.TestId, p => p)
                    .ToDictionary(p => p.Key, p => p.ToList());

                // For each test/group of test...
                foreach (var group in groupByTest)
                {
                    // TODO: Group testresults by requestStart, sorted into groups of 1 second
                    group.Value
                        .OrderBy(p => p.RequestStart)
                        .GroupBy(p => p.RequestStart / TimeSpan.FromSeconds(1).Ticks) // Groups of 1 second
                        .Select(p => new Domain.Entities.TestResultAggregate
                        {
                            Id = p.First().InstanceId,
                            TestId = p.First().TestId,
                            TestRunId = testRun.Id,
                            CreatedAt = p.First().RequestStart,
                            NumSuccessful = p.Count(p => p.TestStatusCode == TestStatusCode.Ok),
                            NumUnsuccessful = p.Count(p => p.TestStatusCode != TestStatusCode.Ok),
                            Min = p.Min(p => p.RequestTime),
                            Max = p.Max(p => p.RequestTime),
                            Average = TimeSpan.FromTicks((long)p.Average(p => p.RequestTime.Ticks))
                        })
                        .ToList()
                        .ForEach(testRun.Results.Add);
                }

                _cacheRepository.PurgeTestRun(testRun.Id);
            }

            testRun.Completed = testRunCompleted;
            testRun.RunEnd = DateTime.Now;

            return await _testRunRepository.CreateAsync(testRun);
        }
    }
}
