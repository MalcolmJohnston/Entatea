using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeleteShortTest")]
    public class SoftDeleteShort
    {
        [KeyType(KeyType.Identity)]
        public int SoftDeleteId { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(1, 0)]
        public short RecordStatus { get; set; }
    }
}
