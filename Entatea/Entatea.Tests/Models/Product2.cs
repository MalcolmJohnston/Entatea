using System;

using Entatea.Annotations;

namespace Entatea.Tests.Models
{
    [Table("Product2s")]
    public class Product2
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? IsForSale { get; set; }

        public DateTime? Updated { get; set; }

        public int? Stock { get; set; }

        public decimal? Price { get; set; }
    }
}
