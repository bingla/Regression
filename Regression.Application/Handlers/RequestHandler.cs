using Regression.Application.Extensions;
using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Models.Results;

namespace Regression.Application.Handlers
{
    public class RequestHandler : DelegatingHandler
    {
        private readonly ICacheRepository _memoryRepository;

        public RequestHandler(ICacheRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Check for RunId and TestId
            if (!Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.RunId) ?? string.Empty, out var runId))
            {
                return GenerateErrorHttpRequestMessage(Globals.HeaderNames.RunId);
            }
            if (!Guid.TryParse(request.GetHeaderValue(Globals.HeaderNames.TestId) ?? string.Empty, out var testId))
            {
                return GenerateErrorHttpRequestMessage(Globals.HeaderNames.TestId);
            }

            // Create test result and add to cache
            var testResult = new TestResult(Guid.NewGuid(), testId, runId, request.RequestUri);
            _memoryRepository.AddTestRun(testResult.RunId, testResult);

            // Add instanceId to header
            request.Headers.Add(Globals.HeaderNames.InstanceId, testResult.InstanceId.ToString());

            return await base.SendAsync(request, cancellationToken);
        }

        private static HttpResponseMessage GenerateErrorHttpRequestMessage(string missingHeaderName)
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Content = new StringContent($"Header '{missingHeaderName}' is required.")
            };
        }
    }
}
