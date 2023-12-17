namespace Regression.Domain.Models.Results
{
    public class ScheduleResult
    {
        public Guid ScheduleId { get; set; }
        public Guid TestCollectionId { get; set; }
        public Guid RunId { get; set; }
        public DateTime ScheduleAt { get; set; }
    }
}
