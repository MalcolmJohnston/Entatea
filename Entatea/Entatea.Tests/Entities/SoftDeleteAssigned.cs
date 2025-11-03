using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeleteAssigned", Schema = "Entatea")]
    public class SoftDeleteAssigned
    {
        [KeyType(KeyType.Assigned)]
        public string Code { get; set; }

        public string Value { get; set; } = "Test";

        [SoftDelete(1, 0)]
        public int RecordStatus { get; set; }
    }
}
