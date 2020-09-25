using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("CitiesSequential")]
    public class CitySequential
    {
        [KeyType(KeyType.Sequential)]
        public short CityId { get; set; }

        [Required]
        public string CityCode { get; set; }

        [Column("Name")]
        [Required]
        public string CityName { get; set; }
    }
}
