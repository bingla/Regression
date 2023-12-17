using Regression.Application.Extensions;
using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Models.Results;

namespace Regression.Application.Handlers
{
    public class ResponseHandler : DelegatingHandler
    {
        private readonly ICacheRepository _memoryRepository;

        public ResponseHandler(ICacheRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // Get request end date
            var requestEnd = DateTime.UtcNow.Ticks;

            // Get data from Headers
            if (Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.InstanceId) ?? string.Empty, out var instanceId) &&
                Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.RunId) ?? string.Empty, out var runId) &&
                Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.TestId) ?? string.Empty, out var testId) &&
                long.TryParse(request.GetHeaderValue(Globals.HeaderNames.RequestStart) ?? string.Empty, out var requestStart))
            {
                // Save the test result to cache
                var testResult = new TestResult
                {
                    InstanceId = instanceId,
                    RunId = runId,
                    TestId = testId,
                    RequestStart = requestStart,
                    RequestEnd = requestEnd,
                    HttpStatusCode = response.StatusCode,
                    TestStatusCode = response.IsSuccessStatusCode
                        ? TestStatusCode.Ok
                        : TestStatusCode.Faulty,
                };

                // Add to cache
                _memoryRepository.AddTestResult(runId, testResult);
            }

            return response;
        }
    }
}
