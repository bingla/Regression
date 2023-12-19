using Regression.Data.Interfaces;
using Regression.Domain;

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
            // Add instanceId to header
            request.Headers.Add(Globals.HeaderNames.InstanceId, Guid.NewGuid().ToString());

            // Add request start tick to header
            request.Headers.Add(Globals.HeaderNames.RequestStart, DateTime.UtcNow.Ticks.ToString());

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
