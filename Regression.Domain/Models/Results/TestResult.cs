using System.Net;

namespace Regression.Domain.Models.Results
{
    public enum TestStatusCode { Ok, Faulty, Incomplete, FaultyHeader };

    public class TestResult
    {
        public Guid InstanceId { get; init; }
        public Guid TestId { get; init; }
        public Guid RunId { get; init; }
        public long RequestStart { get; init; } = DateTime.Now.Ticks;
        public long RequestEnd { get; set; }
        public TimeSpan RequestTime { get { return TimeSpan.FromTicks(RequestEnd) - TimeSpan.FromTicks(RequestStart); } }
        public Uri? Uri { get; init; }
        public HttpStatusCode? HttpStatusCode { get; set; }
        public TestStatusCode TestStatusCode { get; set; } = TestStatusCode.Incomplete;

        public TestResult()
        { }

        public TestResult(Guid instanceId, Guid testId, Guid runId, Uri? uri)
        {
            InstanceId = instanceId;
            TestId = testId;
            RunId = runId;
            Uri = uri;
        }
    }
}
