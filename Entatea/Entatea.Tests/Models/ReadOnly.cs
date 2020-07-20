using Entatea.Annotations;

namespace Entatea.Tests.Models
{
    public class ReadOnly
    {
        [KeyType(KeyType.Sequential)]
        public short SequentialId { get; set; }

        public string Editable { get; set; }

        [ReadOnly(true)]
        [Column("ReadOnly")]
        public string ReadOnlyProperty { get; set; }
    }
}
