using Regression.Application.Interfaces;
using Regression.Domain;
using Regression.Domain.Models.Settings;

namespace Regression.Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly HttpClient _httpClient;

        public RequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Guid testId, RunSettings settings)
        {
            request = AddHeaders(request, testId, settings);
            return await _httpClient.SendAsync(AddHeaders(request, testId, settings), settings.CancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendAsync(Domain.Enums.HttpMethod method, Uri uri, Guid testId, RunSettings settings)
        {
            var request = AddHeaders(new HttpRequestMessage(ParseHttpMethod(method), uri), testId, settings);
            return await SendAsync(request, testId, settings);
        }

        public async Task<HttpResponseMessage> SendAsync(Domain.Enums.HttpMethod method, string url, Guid testId, RunSettings settings)
        {
            var request = AddHeaders(new HttpRequestMessage(ParseHttpMethod(method), url), testId, settings);
            return await SendAsync(request, testId, settings);
        }

        private static HttpRequestMessage AddHeaders(HttpRequestMessage request, Guid testId, RunSettings settings)
        {
            if (!request.Headers.Contains(Globals.HeaderNames.TestId))
                request.Headers.Add(Globals.HeaderNames.TestId, testId.ToString());

            if (!request.Headers.Contains(Globals.HeaderNames.RunId))
                request.Headers.Add(Globals.HeaderNames.RunId, settings.RunId.ToString());

            if (!request.Headers.Contains(Globals.HeaderNames.XApiKey))
                request.Headers.Add(Globals.HeaderNames.XApiKey, settings.XApiKey ?? string.Empty);

            // TODO: Add AuthHeader

            return request;
        }

        private static HttpMethod ParseHttpMethod(Domain.Enums.HttpMethod method)
        {
            return method switch
            {
                Domain.Enums.HttpMethod.POST => HttpMethod.Post,
                Domain.Enums.HttpMethod.PUT => HttpMethod.Put,
                Domain.Enums.HttpMethod.TRACE => HttpMethod.Trace,
                _ => HttpMethod.Get,
            };
        }
    }
}