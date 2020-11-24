using Entatea.Annotations;
using System;

namespace Entatea.Tests.Entities
{
    public class Uid
    {
        [KeyType(KeyType.Identity)]
        public Guid Id { get; set; }

        public string Value { get; set; }
    }
}
