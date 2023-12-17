using System.ComponentModel.DataAnnotations;

namespace Regression.Domain.Entities
{
    public class TestResult
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RunId { get; set; }
        public Guid TestId { get; set; }

        public Uri? Uri { get; set; }
        public string? Url { get; set; }
        public TimeSpan? RequestTime { get; set; }

        public virtual TestRun? Run { get; set; }
        public virtual Test? Test { get; set; }
    }
}
