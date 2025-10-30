using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("HardDeleteShortTest")]
    public class HardDeleteShort
    {
        [KeyType(KeyType.Identity)]
        public int HardDeleteId { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(1, 0)]
        public short RecordStatus { get; set; }
    }
}
