using System;

namespace Entatea.Tests.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsForSale { get; set; }

        public DateTime Updated { get; set; } = DateTime.Now;

        public int Stock { get; set; }

        public decimal Price { get; set; }
    }
}
