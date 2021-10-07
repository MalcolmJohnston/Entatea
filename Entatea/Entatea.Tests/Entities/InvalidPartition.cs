using System;

using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    public class InvalidPartition
    {
        [SequentialPartitionKey(30000, 40000)]
        public int KeyField { get; set; }

        [KeyType(KeyType.Assigned)]
        public int OtherKeyField { get; set; }
    }
}
