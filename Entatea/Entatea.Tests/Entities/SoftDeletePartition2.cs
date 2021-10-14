using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeletePartition")]
    public class SoftDeletePartition2
    {
        [SequentialPartitionKey(50000, true)]
        [Column("SoftDeleteId")]
        public int Id { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(100, 99)]
        public int RecordStatus { get; set; }
    }
}
