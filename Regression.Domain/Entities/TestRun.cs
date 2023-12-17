using System.ComponentModel.DataAnnotations;

namespace Regression.Domain.Entities
{
    /// <summary>
    /// A run that runs all tests in a test collection
    /// </summary>
    public class TestRun
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TestCollectionId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime RunStart { get; set; }
        public DateTime RunEnd { get; set; }
        public bool Completed { get; set; }
        public bool Aborted { get; set; }

        public virtual TestCollection? TestCollection { get; set; }
        public virtual ICollection<TestResult> Results { get; set; } = new HashSet<TestResult>();
    }
}
