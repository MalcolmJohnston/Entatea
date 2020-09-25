using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("AssignedPairAndSequential")]
    public class AssignedPairAndSequential
    {
        [KeyType(KeyType.Assigned)]
        public int FirstAssignedId { get; set; }

        [KeyType(KeyType.Assigned)]
        public int SecondAssignedId { get; set; }

        [KeyType(KeyType.Sequential)]
        public int SequentialId { get; set; }

        [Required]
        [Column("Title")]
        public string Heading { get; set; }
    }
}
