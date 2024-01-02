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
        public Guid TestCollectionId { get; set; }
        public Enums.HttpMethod Method { get; set; } = Enums.HttpMethod.GET;
        public Uri? Uri { get; set; }
        public string? Url { get; set; }

        public virtual TestCollection? TestCollection { get; set; }
        public virtual ICollection<TestResultAggregate> Results { get; init; } = new HashSet<TestResultAggregate>();
    }
}
