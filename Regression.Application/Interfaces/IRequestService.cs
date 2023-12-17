using Regression.Domain.Models.Settings;

namespace Regression.Application.Interfaces
{
    public interface IRequestService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Guid testId, RunSettings settings);
        Task<HttpResponseMessage> SendAsync(HttpMethod method, Uri uri, Guid testId, RunSettings settings);
        Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, Guid testId, RunSettings settings);
    }
}
