using TdDb.Data;
using TdDb.DataAnnotations;

namespace TdDb.Tests.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        public string CityCode { get; set; }

        [Column("Name")]
        [Required]
        public string CityName { get; set; }

        [Required]
        public string Area { get; set; }
    }
}
