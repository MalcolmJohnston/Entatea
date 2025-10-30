using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("HardDeletePartition")]
    public class HardDeletePartition1
    {
        [SequentialPartitionKey(20000, 49999)]
        [Column("HardDeleteId")]
        public int Id { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(100, 99)]
        public int RecordStatus { get; set; }
    }
}
