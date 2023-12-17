using System.ComponentModel.DataAnnotations;

namespace Regression.Domain.Entities
{
    /// <summary>
    /// A test in a test collection.
    /// </summary>
    public class Test
    {
        [Key]
        public Guid Id { get; set; }
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Uri? Uri { get; set; }
        public string? Url { get; set; }

        public virtual TestCollection? TestCollection { get; set; }
        public virtual ICollection<TestResult> Results { get; init; } = new HashSet<TestResult>();
    }
}
