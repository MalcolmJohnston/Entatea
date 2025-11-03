using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeleteShortTest")]
    public class SoftDeleteShortNoAttribute
    {
        [KeyType(KeyType.Identity)]
        public int SoftDeleteId { get; set; }

        public string Value { get; set; } = "Test";

        public short RecordStatus { get; set; }
    }
}
