namespace Regression.Domain.Entities
{
    public class TestResultAggregate
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Guid TestRunId { get; set; }
        public long CreatedAt { get; set; }
        public int NumSuccessful { get; set; }
        public int NumUnsuccessful { get; set; }
        public TimeSpan Min { get; set; }
        public TimeSpan Max { get; set; }
        public TimeSpan Average { get; set; }

        public virtual Test? Test { get; set; }
        public virtual TestRun? TestRun { get; set; }
    }
}
