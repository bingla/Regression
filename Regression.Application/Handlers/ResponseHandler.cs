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

            // Handle response
            // Get request end date
            var requestEnd = DateTime.Now.Ticks;

            // Var get testId and runId from request header
            _ = Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.InstanceId) ?? string.Empty, out var instanceId);
            _ = Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.RunId) ?? string.Empty, out var runId);

            // Update the test end time and result
            var testResult = _memoryRepository.GetTestResult(runId, instanceId);
            if (testResult != default)
            {
                testResult.RequestEnd = requestEnd;
                testResult.HttpStatusCode = response.StatusCode;
                testResult.TestStatusCode = response.IsSuccessStatusCode
                    ? TestStatusCode.Ok
                    : TestStatusCode.Faulty;

                // Update cache
                _memoryRepository.AddTestRun(runId, testResult);
            }

            return response;
        }
    }
}
