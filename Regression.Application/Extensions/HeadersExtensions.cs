using System.Net.Http.Headers;

namespace Regression.Application.Extensions
{
    public static class HeadersExtensions
    {
        public static string? GetHeaderValue(this HttpRequestMessage request, string headerName)
        {
            return request != default
                ? request.Headers.GetHeaderValue(headerName)
                : default;
        }

        public static string? GetHeaderValue(this HttpRequestHeaders headers, string headerName)
        {
            return headers != default && headers.TryGetValues(headerName, out var result)
                ? result.FirstOrDefault()
                : default;
        }
    }
}
