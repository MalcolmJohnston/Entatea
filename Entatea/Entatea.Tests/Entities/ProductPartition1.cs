using System;

using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("Products")]
    public class ProductPartition1
    {
        [KeyType(KeyType.Sequential)]
        [SequentialPartitionKey(100000, false)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsForSale { get; set; }

        public DateTime Updated { get; set; } = DateTime.Now;

        public int Stock { get; set; }

        public decimal Price { get; set; }
    }
}
