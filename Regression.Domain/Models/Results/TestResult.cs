﻿using System.Net;

namespace Regression.Domain.Models.Results
{
    public enum TestStatusCode { Ok, Faulty, Incomplete, FaultyHeader };

    public class TestResult
    {
        public Guid InstanceId { get; init; }
        public Guid TestId { get; init; }
        public Guid RunId { get; init; }
        public long RequestStart { get; init; } = DateTime.UtcNow.Ticks;
        public long RequestEnd { get; set; }
        public TimeSpan RequestTime { get { return TimeSpan.FromTicks(RequestEnd) - TimeSpan.FromTicks(RequestStart); } }
        public Uri? Uri { get; init; }
        public HttpStatusCode? HttpStatusCode { get; set; }
        public TestStatusCode TestStatusCode { get; set; } = TestStatusCode.Incomplete;
    }
}
