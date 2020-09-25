using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("SoftDeleteTest", Schema = "TdDb")]
    public class SoftDelete
    {
        [KeyType(KeyType.Identity)]
        public int SoftDeleteId { get; set; }

        [SoftDelete(1, 0)]
        public int RecordStatus { get; set; }
    }
}
