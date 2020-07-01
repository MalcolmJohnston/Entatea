using TdDb.Data;
using TdDb.DataAnnotations;

namespace TdDb.Tests.Models
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
