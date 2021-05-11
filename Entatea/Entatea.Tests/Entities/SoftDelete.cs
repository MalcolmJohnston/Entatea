using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeleteTest", Schema = "Entatea")]
    public class SoftDelete
    {
        [KeyType(KeyType.Identity)]
        public int SoftDeleteId { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(1, 0)]
        public int RecordStatus { get; set; }
    }
}
