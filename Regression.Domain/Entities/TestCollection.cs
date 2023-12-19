using System.ComponentModel.DataAnnotations;
using Environment = Regression.Domain.Enums.Environment;

namespace Regression.Domain.Entities
{
    /// <summary>
    /// Test collection that contains tests
    /// </summary>
    public class TestCollection
    {
        [Key]
        public Guid Id { get; set; }
        public Environment Environment { get; set; } = Environment.None;
        public int NumIterations { get; set; } = 1;
        public int NumAgents { get; set; } = 1;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AppId { get; set; }
        public string? AppSecret { get; set; }
        public string? XApiKey { get; set; }

        public virtual ICollection<TestRun> Runs { get; init; } = new HashSet<TestRun>();
        public virtual ICollection<Test> Tests { get; init; } = new HashSet<Test>();
        public virtual ICollection<TestResult> Results { get; init; } = new HashSet<TestResult>();
    }
}
