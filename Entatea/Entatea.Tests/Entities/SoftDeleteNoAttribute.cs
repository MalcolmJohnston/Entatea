using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeleteTest", Schema = "Entatea")]
    public class SoftDeleteNoAttribute
    {
        [KeyType(KeyType.Identity)]
        public int SoftDeleteId { get; set; }

        public string Value { get; set; } = "Test";

        public int RecordStatus { get; set; }
    }
}
