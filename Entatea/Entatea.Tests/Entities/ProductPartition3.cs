﻿using System;

using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("Products")]
    public class ProductPartition3
    {
        [KeyType(KeyType.Sequential)]
        [SequentialPartitionKey(30000, 40000)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsForSale { get; set; }

        public DateTime Updated { get; set; } = DateTime.Now;

        public int Stock { get; set; }

        public decimal Price { get; set; }
    }
}
