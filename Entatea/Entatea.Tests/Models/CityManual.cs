using Entatea.Annotations;

namespace Entatea.Tests.Models
{
    [Table("CitiesManual")]
    public class CityManual
    {
        [KeyType(KeyType.Assigned)]
        [Required]
        public string CityCode { get; set; }

        [Column("Name")]
        [Required]
        public string CityName { get; set; }
    }
}
