using TdDb.Data;
using TdDb.DataAnnotations;

namespace TdDb.Tests.Models
{
    public class AssignedAndSequential
    {
        [KeyType(KeyType.Assigned)]
        public int AssignedId { get; set; }

        [KeyType(KeyType.Sequential)]
        public int SequentialId { get; set; }

        [Required]
        [Column("Title")]
        public string Heading { get; set; }
    }
}
