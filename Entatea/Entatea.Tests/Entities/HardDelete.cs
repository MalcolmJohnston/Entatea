using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("HardDeleteTest", Schema = "Entatea")]
    public class HardDelete
    {
        [KeyType(KeyType.Identity)]
        public int HardDeleteId { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(1, 0)]
        public int RecordStatus { get; set; }
    }
}
