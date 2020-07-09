using System;

using Testadal.Annotations;

namespace Testadal.Tests.Models
{
    [Table("DateStampTest")]
    public class DateStamp
    {
        [KeyType(KeyType.Assigned)]
        public string Name { get; set; }

        public string Value { get; set; }

        [DateStamp]
        [ReadOnly(true)]
        public DateTime InsertDate { get; set; }

        [DateStamp]
        public DateTime UpdateDate { get; set; }
    }
}
