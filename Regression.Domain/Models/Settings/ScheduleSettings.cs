namespace Regression.Domain.Models.Settings
{
    public class ScheduleSettings
    {
        public Guid ScheduleId { get; set; }
        public Guid TestCollectionId { get; set; }
        public ICollection<ScheduleItem> ScheduleAt { get; set; } = new List<ScheduleItem>();
        public bool Recurring { get; set; }
        public bool Enabled { get; set; }
    }
}
