using Regression.Domain.Models.Settings;
using System.ComponentModel.DataAnnotations;

namespace Regression.Domain.Entities
{
    public class Schedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TestCollectionId { get; set; }
        public ICollection<ScheduleItem> ScheduleAt { get; init; } = new List<ScheduleItem>();
        public bool Recurring { get; set; }
        public bool Enabled { get; set; }

        public virtual TestCollection? TestCollection { get; set; }
        public virtual ICollection<TestRun> TestRuns { get; init; } = new HashSet<TestRun>();
    }
}
