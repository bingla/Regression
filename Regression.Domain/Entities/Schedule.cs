using System.ComponentModel.DataAnnotations;
using Regression.Domain.Models.Settings;

namespace Regression.Domain.Entities
{
    public class Schedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TestCollectionId { get; set; }
        public ICollection<ScheduleItem> ScheduleAt { get; set; } = new List<ScheduleItem>();
        public bool Recurring { get; set; }
        public bool Enabled { get; set; }

        public virtual TestCollection? TestCollection { get; set; }
    }
}
